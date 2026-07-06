#!/bin/bash
# TaskFlow NoteManager — Launcher
PROJECT_DIR="/home/tony/Projetos/gerenciador_de_tarefas_e_notas/src/TaskFlow.Presentation"
PORT=5000
URL="http://localhost:$PORT"

echo "🚀 TaskFlow NoteManager"
echo "=========================="

# Se o servidor já está rodando, só abre o navegador
if curl -s -o /dev/null -w "%{http_code}" "$URL" 2>/dev/null | grep -qE "200|302|404"; then
    echo "✅ Servidor já está rodando em $URL"
    xdg-open "$URL" 2>/dev/null &
    exit 0
fi

echo "📦 Iniciando servidor em http://localhost:$PORT..."
cd "$PROJECT_DIR" || { echo "❌ Diretório não encontrado"; exit 1; }
dotnet run --urls "http://localhost:$PORT" &
sleep 4
echo "✅ Servidor iniciado!"
xdg-open "$URL" 2>/dev/null &
wait
