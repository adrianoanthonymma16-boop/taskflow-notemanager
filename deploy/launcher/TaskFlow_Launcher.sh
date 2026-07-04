#!/bin/bash
# ============================================================
# TaskFlow NoteManager — Desktop Launcher (Linux)
# Clique neste arquivo para iniciar o app e abrir no navegador.
# ============================================================

APP_DIR="$HOME/.local/share/TaskFlow/App"
APP_DLL="$APP_DIR/TaskFlow.Presentation.dll"
PORT=5000
URL="http://localhost:$PORT"
LOCK_FILE="/tmp/taskflow.lock"

# Verifica se o app já está rodando
if [ -f "$LOCK_FILE" ]; then
    PID=$(cat "$LOCK_FILE" 2>/dev/null)
    if kill -0 "$PID" 2>/dev/null; then
        # App já está rodando — só abre o navegador
        xdg-open "$URL" 2>/dev/null || sensible-browser "$URL" 2>/dev/null || firefox "$URL" 2>/dev/null &
        exit 0
    fi
fi

# Remove lock file antigo
rm -f "$LOCK_FILE"

# Inicia o app em background
if [ -f "$APP_DLL" ]; then
    cd "$APP_DIR" || exit 1
    nohup dotnet "$APP_DLL" --urls "http://localhost:$PORT" > /tmp/taskflow.log 2>&1 &
    APP_PID=$!
    echo $APP_PID > "$LOCK_FILE"

    # Aguarda o servidor iniciar (até 15 segundos)
    for i in $(seq 1 30); do
        if curl -s -o /dev/null -w "%{http_code}" "$URL" 2>/dev/null | grep -q "200\|302\|404"; then
            break
        fi
        sleep 0.5
    done

    # Abre o navegador
    xdg-open "$URL" 2>/dev/null || sensible-browser "$URL" 2>/dev/null || firefox "$URL" 2>/dev/null &
else
    # Fallback: tenta rodar a partir do diretório do projeto
    echo "Aplicativo não encontrado em $APP_DIR."
    echo "Certifique-se de que o TaskFlow está instalado ou execute 'dotnet run' no diretório do projeto."
    read -p "Pressione Enter para sair..."
    exit 1
fi
