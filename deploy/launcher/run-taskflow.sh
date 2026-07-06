#!/bin/bash
# TaskFlow NoteManager — Launcher v3.0 (force reload)
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR/../../src/TaskFlow.Presentation"
SOLUTION_DIR="$SCRIPT_DIR/../.."
PORT=5000
URL="http://localhost:$PORT/tasks"

echo "🚀 TaskFlow NoteManager v3.0"
echo "=========================="

if [ ! -d "$PROJECT_DIR" ]; then
    echo "❌ Projeto não encontrado: $PROJECT_DIR"
    exit 1
fi

echo "🛑 Parando servidor anterior..."
fuser -k $PORT/tcp 2>/dev/null
sleep 1

echo "🔨 Recompilando..."
cd "$SOLUTION_DIR" || exit 1
dotnet build --nologo -v q 2>&1 | tail -1
if [ ${PIPESTATUS[0]} -ne 0 ]; then
    echo "❌ Erro na compilação. Execute 'dotnet build' para ver detalhes."
    exit 1
fi

echo "📦 Iniciando servidor..."
cd "$PROJECT_DIR" || exit 1
dotnet run --urls "http://localhost:$PORT" &
sleep 5
echo "✅ Servidor iniciado — abrindo aba Tarefas..."
xdg-open "$URL" 2>/dev/null &
wait
