#!/bin/bash
# TaskFlow NoteManager — Launcher v2.0
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR/../../src/TaskFlow.Presentation"
PORT=5000
URL="http://localhost:$PORT/tasks"

echo "🚀 TaskFlow NoteManager v2.0"
echo "=========================="

if [ ! -d "$PROJECT_DIR" ]; then
    echo "❌ Projeto não encontrado: $PROJECT_DIR"
    echo "   Execute o launcher dentro do diretório do projeto."
    exit 1
fi

if curl -s -o /dev/null -w "%{http_code}" "$URL" 2>/dev/null | grep -qE "200|302|404"; then
    echo "✅ Servidor já está rodando"
    xdg-open "$URL" 2>/dev/null &
    exit 0
fi

echo "📦 Verificando build..."
if [ ! -f "$PROJECT_DIR/bin/Debug/net8.0/TaskFlow.Presentation.dll" ]; then
    echo "🔨 Compilando o projeto (primeira vez)..."
    cd "$SCRIPT_DIR/../../" || exit 1
    dotnet build -q 2>&1 || { echo "❌ Erro ao compilar. Execute 'dotnet build' manualmente."; exit 1; }
fi

echo "📦 Iniciando servidor..."
cd "$PROJECT_DIR" || exit 1
dotnet run --urls "http://localhost:$PORT" &
sleep 4
echo "✅ Servidor iniciado em http://localhost:$PORT"
echo "   Abrindo aba Tarefas..."
xdg-open "$URL" 2>/dev/null &
wait
