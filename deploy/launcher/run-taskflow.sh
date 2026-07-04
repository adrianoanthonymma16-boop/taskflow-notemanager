#!/bin/bash
# TaskFlow NoteManager — Dev Launcher
PROJECT_DIR="/home/tony/Projetos/gerenciador_de_tarefas_e_notas/src/TaskFlow.Presentation"
PORT=5000
URL="http://localhost:$PORT"

# Se o servidor já está rodando, só abre o navegador
if curl -s -o /dev/null -w "%{http_code}" "$URL" 2>/dev/null | grep -qE "200|302|404"; then
    xdg-open "$URL" 2>/dev/null &
    exit 0
fi

# Inicia o servidor em background e abre o navegador
cd "$PROJECT_DIR" || exit 1
dotnet run --urls "http://localhost:$PORT" &
sleep 3
xdg-open "$URL" 2>/dev/null &
wait
