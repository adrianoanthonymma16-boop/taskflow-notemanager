# TaskFlow NoteManager

[English](#english) | [Português](#português)

---

## English

**TaskFlow NoteManager** is a **desktop offline application** for task and note management, running entirely on the user's machine without any internet dependency. It opens in the local browser via Blazor Server and stores all data in a local SQLite database.

### Problems it solves

| Pain Point | Solution |
|---|---|
| No internet available at the workspace | 100% offline — runs on `localhost:5000`, no cloud dependency |
| Need for task tracking with deadlines | Full CRUD with due dates, status flow (Open → Pending → Completed), and overdue warnings |
| Tasks blocked by third parties | Pending logs with reason, counterparty name, signature capture, and resolution tracking |
| Unstructured notes scattered across files | Centralized notes with title, content, status (Draft/Published/Archived), and file attachments |
| Document attachments mixed with notes | File upload (PDF, DOC, DOCX) attached directly to notes, isolated per user |
| Data portability between machines | PDF export with MigrationId (TF-timestamp), JSON data + visual report with summary bar |
| Cross-platform desktop usage | Single self-contained executable — works on Windows and Linux |
| Multiple users on the same machine | Per-user accounts with BCrypt password hashing, security questions for recovery |
| Complex tools for simple workflows | Clean HTML-native modals, native browser form elements, one-click actions |

### Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 12 / .NET 8 LTS |
| Framework | Blazor Server (interactive via SignalR, CircuitOptions configured) |
| Database | SQLite (EF Core 8) |
| UI — Dialogs | 100% native HTML + CSS inline modals (zero MudBlazor in dialogs) |
| UI — Layout | MudBlazor 8.5 (AppBar, Drawer, NavMenu, Snackbar) |
| PDF Generation | QuestPDF 2024.12 |
| Password Hashing | BCrypt.Net-Next |
| Object Mapping | AutoMapper 12.0 |
| Testing | xUnit + Moq + FluentAssertions (16 tests) |
| Dark Mode | CSS theme with localStorage persistence |

### Architecture — Clean Architecture

```
Presentation (Blazor Server)
    ↓ depends on
Application (Use cases, DTOs, Service interfaces)
    ↓ depends on
Domain (Entities, Enums, Repository interfaces)
    ↑ implements
Infrastructure (EF Core DbContext, Repositories, PDF services, FileStorage)
```

| Project | Responsibility |
|---|---|
| `TaskFlow.Domain` | Entities (`User`, `TaskItem`, `PendingLog`, `Note`, `Attachment`, `VisualPdfRecord`), Enums, Repository interfaces |
| `TaskFlow.Application` | DTOs, Service interfaces, Implementations (Auth, Task, Note, Export, Import), AutoMapper |
| `TaskFlow.Infrastructure` | `AppDbContext` (SQLite), Repository implementations, `FileStorageService` (user-isolated), QuestPDF generators, DI |
| `TaskFlow.Presentation` | Blazor Server pages with HTML-native inline modals, MudBlazor layout, JS interop, CSS theme system |

### Features

- **Identity**: Registration, login, password recovery via security question, profile editing with photo
- **TaskFlow (Tasks)**:
  - CRUD with title, description, due date
  - **Inline view modal** with full details, pending history, signatures, actions
  - **Pending system**: create multiple pending logs per task with reason, counterparty
  - **Sanar pendências**: clickable dialog lists all active pendencies; resolve specific or all
  - **Resolution note**: optional text explaining measures taken, saved in DB and visible in PDF
  - **Individual resolution**: resolve each pending log independently; task reopens when all resolved
  - Status flow: Open → Pending → Completed (with reopen)
  - Block completion when active pending exists, with **force-complete confirmation** dialog
  - Filter by status
  - Overdue warnings on cards and Dashboard
- **NoteManager (Notes)**:
  - Clickable note cards open full detail modal
  - CRUD with title, content, status (Draft/Published/Archived)
  - File upload attachment system (PDF, DOC, DOCX) saved in user-isolated directory
  - Attachment listing with file size formatting
- **Export**:
  - **Type 1 — Data PDF**: embedded JSON with MigrationId for machine transfer/backup
  - **Type 2 — Visual PDF**: comprehensive report with summary bar, inline pending logs per task (active/resolved with color coding), signatures, migration metadata
  - Scope selection (Tasks only, Notes only, or Full System)
  - **Month filtering**: export all data or select specific months via checkboxes; filtered period shown in PDF header
  - PDF opens in browser tab + downloads
- **Import**:
  - **Mode A — Data import**: restore from Type 1 PDFs
    - Conflict strategies: Replace, Merge, Append
    - Date filtering for selective import
    - External ownership detection
  - **Mode B — Visual import**: store any PDF for offline reading
- **Dashboard**: Task counts, note count, overdue list, weekly deadlines, status distribution
- **Dark Mode**: CSS-based theme with localStorage persistence (light/dark toggle)
- **Single Instance**: Cross-platform Mutex + .lock file prevents duplicate app execution
- **SignalR Production Config**: CircuitOptions (5min retention, 120s JS timeout), HubOptions (64MB msg, 15s keepalive)

### Dependencies

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A modern web browser (Chrome, Firefox, Edge, etc.)
- **Linux only**: `curl` (usually pre-installed)

### Running

```bash
# Clone
git clone https://github.com/adrianoanthonymma16-boop/taskflow-notemanager.git
cd taskflow-notemanager

# Run
cd src/TaskFlow.Presentation
dotnet run

# Opens at http://localhost:5000
```

### Desktop Shortcut (Linux)

The launcher kills any previous server, cleans the build cache, rebuilds, and starts fresh every time:

```bash
cat > ~/Desktop/taskflow.desktop << 'EOF'
[Desktop Entry]
Version=2.0
Type=Application
Name=TaskFlow NoteManager
Comment=Gerenciador de tarefas e notas offline
Exec=SEU_CAMINHO/deploy/launcher/run-taskflow.sh
Icon=SEU_CAMINHO/src/TaskFlow.Presentation/wwwroot/taskflow-icon.svg
Terminal=false
Categories=Office;Utility;
StartupNotify=true
StartupWMClass=TaskFlow
EOF

chmod +x ~/Desktop/taskflow.desktop
gio set ~/Desktop/taskflow.desktop metadata::trusted true
```

> Replace `SEU_CAMINHO` with your absolute project path (e.g., `/home/tony/Projetos/gerenciador_de_tarefas_e_notas`).
> The SVG icon is auto-generated at `src/TaskFlow.Presentation/wwwroot/taskflow-icon.svg`.

#### Windows

Create a shortcut to `deploy\launcher\TaskFlow_Launcher.bat`:

1. Right-click on your Desktop → **New → Shortcut**
2. Browse to `C:\Users\%USERNAME%\...\deploy\launcher\TaskFlow_Launcher.bat`
3. Name it `TaskFlow NoteManager`

Or via PowerShell:

```powershell
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut("$env:USERPROFILE\Desktop\TaskFlow.lnk")
$Shortcut.TargetPath = "$env:USERPROFILE\Projetos\gerenciador_de_tarefas_e_notas\deploy\launcher\TaskFlow_Launcher.bat"
$Shortcut.IconLocation = "imageres.dll,12"
$Shortcut.Save()
```

### Testing

```bash
dotnet test
```

### Deploy

```bash
# Windows (requires Inno Setup)
dotnet publish src/TaskFlow.Presentation -c Release -r win-x64 --self-contained
iscc deploy/windows/installer.iss

# Linux (.deb)
bash deploy/linux/build-deb.sh
```

---

## License

This project is licensed under the [MIT License](LICENSE).

---

© 2024 TaskFlow NoteManager. Distributed under the MIT License.

---

## Português

**TaskFlow NoteManager** é um **aplicativo desktop offline** para gerenciamento de tarefas e notas, rodando inteiramente na máquina do usuário sem dependência de internet. Abre no navegador local via Blazor Server e armazena todos os dados em um banco SQLite local.

### Dores que resolve

| Dor | Solução |
|---|---|
| Sem internet no local de trabalho | 100% offline — roda em `localhost:5000`, sem dependência de nuvem |
| Necessidade de acompanhar tarefas com prazos | CRUD completo com datas de prazo, fluxo de status (Aberta → Pendente → Concluída) e avisos de atraso |
| Tarefas bloqueadas por terceiros | Logs de pendência com motivo, contraparte, captura de assinatura e rastreamento de resolução |
| Notas desestruturadas espalhadas em arquivos | Notas centralizadas com título, conteúdo e status (Rascunho/Publicado/Arquivado) e anexos |
| Documentos anexos misturados com notas | Upload de arquivos (PDF, DOC, DOCX) anexados diretamente às notas, isolados por usuário |
| Portabilidade de dados entre máquinas | Exportação PDF com MigrationId (TF-timestamp), JSON + relatório visual com barra de resumo |
| Uso em múltiplos sistemas operacionais | Executável auto-contido único — funciona no Windows e Linux |
| Múltiplos usuários na mesma máquina | Contas por usuário com hash BCrypt, pergunta de segurança para recuperação |
| Ferramentas complexas para fluxos simples | Modais HTML nativos, elementos de formulário nativos, ações com um clique |

### Pilha Tecnológica

| Camada | Tecnologia |
|---|---|
| Linguagem | C# 12 / .NET 8 LTS |
| Framework | Blazor Server (interativo via SignalR, CircuitOptions configurado) |
| Banco de dados | SQLite (EF Core 8) |
| UI — Dialogs | 100% HTML nativo + CSS inline (zero MudBlazor nos modais) |
| UI — Layout | MudBlazor 8.5 (AppBar, Drawer, NavMenu, Snackbar) |
| Geração de PDF | QuestPDF 2024.12 |
| Hash de senhas | BCrypt.Net-Next |
| Mapeamento de objetos | AutoMapper 12.0 |
| Testes | xUnit + Moq + FluentAssertions (16 testes) |
| Modo Escuro | Tema CSS com persistência localStorage |

### Arquitetura — Clean Architecture

```
Presentation (Blazor Server)
    ↓ depende de
Application (Casos de uso, DTOs, Interfaces de serviço)
    ↓ depende de
Domain (Entidades, Enums, Interfaces de repositório)
    ↑ implementa
Infrastructure (EF Core DbContext, Repositórios, Serviços PDF, FileStorage)
```

| Projeto | Responsabilidade |
|---|---|
| `TaskFlow.Domain` | Entidades (`User`, `TaskItem`, `PendingLog`, `Note`, `Attachment`, `VisualPdfRecord`), Enums, Interfaces de repositório |
| `TaskFlow.Application` | DTOs, Interfaces de serviço, Implementações (Auth, Task, Note, Export, Import), AutoMapper |
| `TaskFlow.Infrastructure` | `AppDbContext` (SQLite), Repositórios, `FileStorageService` (isolado por usuário), Geradores QuestPDF, DI |
| `TaskFlow.Presentation` | Páginas Blazor Server com modais inline HTML nativos, Layout MudBlazor, Interop JS, Sistema de temas CSS |

### Funcionalidades

- **Identidade**: Cadastro, login, recuperação de senha via pergunta de segurança, edição de perfil com foto
- **TaskFlow (Tarefas)**:
  - CRUD com título, descrição, data de término
  - **Modal de visualização inline** com detalhes completos, histórico de pendências, assinaturas, ações
  - **Sistema de pendências**: múltiplas pendências por tarefa com motivo e contraparte
  - **Sanar pendências**: diálogo clicável lista todas as pendências ativas; resolva específica ou todas
  - **Nota de resolução**: texto opcional sobre providências tomadas, salvo no banco e visível no PDF
  - **Resolução individual**: resolva cada pendência separadamente; tarefa reabre quando todas resolvidas
  - Fluxo de status: Aberta → Pendente → Concluída (com reabertura)
  - Bloqueio de conclusão quando há pendência ativa, com **diálogo de confirmação** para forçar conclusão
  - Filtro por status
  - Avisos de atraso nos cards e Dashboard
- **NoteManager (Notas)**:
  - Cards clicáveis abrem modal com detalhes completos
  - CRUD com título, conteúdo, status (Rascunho/Publicado/Arquivado)
  - Upload de anexos (PDF, DOC, DOCX) salvos em diretório isolado por usuário
  - Listagem de anexos com formatação de tamanho
- **Exportação**:
  - **Tipo 1 — PDF de Dados**: JSON embutido com MigrationId para transferência/backup
  - **Tipo 2 — PDF Visual**: relatório completo com barra de resumo, pendências inline por tarefa (ativas/resolvidas com código de cores), assinaturas, metadados
  - Seleção de escopo (Apenas Tarefas, Apenas Notas, ou Sistema Completo)
  - **Filtro por meses**: exportar tudo ou selecionar meses específicos via checkboxes; período filtrado exibido no cabeçalho do PDF
  - PDF abre no navegador + faz download
- **Importação**:
  - **Modo A — Importação de dados**: restaurar de PDFs Tipo 1
    - Estratégias de conflito: Substituir, Mesclar, Anexar
    - Filtro por data para importação seletiva
    - Detecção de propriedade externa
  - **Modo B — Importação visual**: armazenar qualquer PDF para leitura offline
- **Dashboard**: Contadores, tarefas atrasadas, prazos da semana, distribuição de status
- **Modo Escuro**: Tema CSS com persistência localStorage (alternância claro/escuro)
- **Instância Única**: Mutex cross-platform + arquivo .lock impede execução duplicada
- **SignalR Produção**: CircuitOptions (5min retenção, 120s timeout JS), HubOptions (64MB msg, 15s keepalive)

### Dependências

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- Um navegador moderno (Chrome, Firefox, Edge, etc.)
- **Apenas Linux**: `curl` (geralmente já instalado)

### Executando

```bash
# Clonar
git clone https://github.com/adrianoanthonymma16-boop/taskflow-notemanager.git
cd taskflow-notemanager

# Executar
cd src/TaskFlow.Presentation
dotnet run

# Abre em http://localhost:5000
```

### Atalho na Área de Trabalho (Linux)

O launcher mata o servidor anterior, limpa o cache de build, recompila e inicia do zero a cada clique:

```bash
cat > ~/Desktop/taskflow.desktop << 'EOF'
[Desktop Entry]
Version=2.0
Type=Application
Name=TaskFlow NoteManager
Comment=Gerenciador de tarefas e notas offline
Exec=SEU_CAMINHO/deploy/launcher/run-taskflow.sh
Icon=SEU_CAMINHO/src/TaskFlow.Presentation/wwwroot/taskflow-icon.svg
Terminal=false
Categories=Office;Utility;
StartupNotify=true
StartupWMClass=TaskFlow
EOF

chmod +x ~/Desktop/taskflow.desktop
gio set ~/Desktop/taskflow.desktop metadata::trusted true
```

> Substitua `SEU_CAMINHO` pelo caminho absoluto do projeto (ex: `/home/tony/Projetos/gerenciador_de_tarefas_e_notas`).
> O ícone SVG está em `src/TaskFlow.Presentation/wwwroot/taskflow-icon.svg`.

#### Windows

Crie um atalho para `deploy\launcher\TaskFlow_Launcher.bat`:

1. Clique com botão direito na Área de Trabalho → **Novo → Atalho**
2. Navegue até `C:\Users\%USERNAME%\...\deploy\launcher\TaskFlow_Launcher.bat`
3. Nomeie como `TaskFlow NoteManager`

Ou via PowerShell:

```powershell
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut("$env:USERPROFILE\Desktop\TaskFlow.lnk")
$Shortcut.TargetPath = "$env:USERPROFILE\Projetos\gerenciador_de_tarefas_e_notas\deploy\launcher\TaskFlow_Launcher.bat"
$Shortcut.IconLocation = "imageres.dll,12"
$Shortcut.Save()
```

### Testes

```bash
dotnet test
```

### Deploy

```bash
# Windows (requer Inno Setup)
dotnet publish src/TaskFlow.Presentation -c Release -r win-x64 --self-contained
iscc deploy/windows/installer.iss

# Linux (.deb)
bash deploy/linux/build-deb.sh
```
