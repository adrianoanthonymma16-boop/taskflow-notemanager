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
| Tasks blocked by third parties | Pending logs with reason, counterparty name, and resolution tracking |
| Unstructured notes scattered across files | Centralized notes with title, content, status (Draft/Published/Archived) |
| Document attachments mixed with notes | File upload (PDF, DOC, DOCX) attached directly to notes |
| Data portability between machines | PDF export with embedded JSON data + visual printable PDF reports |
| Cross-platform desktop usage | Single self-contained executable — works on Windows and Linux |
| Multiple users on the same machine | Per-user accounts with BCrypt password hashing, security questions for recovery |
| Complex tools for simple workflows | Clean, card-based UI; one-click actions; native browser date pickers and selects |

### Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 12 / .NET 8 LTS |
| Framework | Blazor Server (interactive via SignalR) |
| Database | SQLite (EF Core 8) |
| UI Library | MudBlazor 8.5 |
| PDF Generation | QuestPDF |
| Password Hashing | BCrypt.Net-Next |
| Object Mapping | AutoMapper |
| Testing | xUnit + Moq + FluentAssertions |

### Architecture — Clean Architecture

```
Presentation (Blazor Server)
    ↓ depends on
Application (Use cases, DTOs, Service interfaces)
    ↓ depends on
Domain (Entities, Enums, Repository interfaces)
    ↑ implements
Infrastructure (EF Core DbContext, Repositories, PDF services)
```

| Project | Responsibility |
|---|---|
| `TaskFlow.Domain` | Entities (`User`, `TaskItem`, `PendingLog`, `Note`, `Attachment`, `VisualPdfRecord`), Enums, Repository interfaces |
| `TaskFlow.Application` | DTOs, Service interfaces (`IAuthService`, `ITaskService`, `INoteService`, `IExportService`, `IImportService`), Implementations, AutoMapper |
| `TaskFlow.Infrastructure` | `AppDbContext` (SQLite), Repository implementations, `FileStorageService`, QuestPDF generators, DI configuration |
| `TaskFlow.Presentation` | Blazor Server pages, MudBlazor layout, JS interop, theme system |

### Features

- **Identity**: Registration, login, password recovery via security question, profile editing with photo
- **TaskFlow (Tasks)**:
  - CRUD with title, description, due date
  - Status flow: Open → Pending → Completed (with reopen)
  - Pending logs with reason and counterparty
  - Resolve pending (reopens task)
  - Filter by status
  - Overdue warnings on cards and Dashboard
  - Dashboard with statistics, overdue tasks, weekly deadlines, status distribution
- **NoteManager (Notes)**:
  - CRUD with title, content, status (Draft/Published/Archived)
  - File upload attachment system (PDF, DOC, DOCX)
  - Attachment listing with file size formatting
- **Export**:
  - **Type 1 — Data PDF**: embedded JSON for machine transfer/backup
  - **Type 2 — Visual PDF**: formatted tables and full note content for printing
  - Scope selection (Tasks only, Notes only, or Full System)
- **Import**:
  - **Mode A — Data import**: restore from Type 1 PDFs
    - Conflict strategies: Replace, Merge, Append
    - Date filtering for selective import
    - External ownership detection with Integrate/Visual/Cancel options
  - **Mode B — Visual import**: store any PDF for offline reading
- **Dashboard**: Task counts (Open/Pending/Completed), note count, overdue list, weekly deadlines, status distribution bars, recent tasks and notes
- **Dark Mode**: Native CSS-based gray-tone dark theme with localStorage persistence
- **Single Instance**: Global Mutex prevents duplicate app execution

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

# Open browser at
# http://localhost:5000
```

### Desktop Shortcuts

#### Linux

Create a `.desktop` file on your desktop:

```bash
cat > ~/Desktop/taskflow-notemanager.desktop << 'EOF'
[Desktop Entry]
Type=Application
Name=TaskFlow NoteManager
Comment=Gerenciador de tarefas e notas offline
Exec=/home/$USER/Projetos/gerenciador_de_tarefas_e_notas/deploy/launcher/run-taskflow.sh
Icon=text-editor
Terminal=false
Categories=Office;Utility;
Keywords=task;note;manager;pdf;signature;offline;
StartupWMClass=taskflow
EOF

chmod +x ~/Desktop/taskflow-notemanager.desktop
```

> Adjust `Exec=` to point to `deploy/launcher/run-taskflow.sh` inside your project directory.
> Right-click the icon on your desktop and select **"Allow Launching"** if prompted.

#### Windows

Create a shortcut to `deploy\launcher\TaskFlow_Launcher.bat`:

1. Right-click on your Desktop → **New → Shortcut**
2. Browse to `C:\Users\%USERNAME%\...\deploy\launcher\TaskFlow_Launcher.bat`
3. Name it `TaskFlow NoteManager`
4. Right-click the shortcut → **Properties → Change Icon** (optional)

Or via command line (PowerShell):

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

## Licença

Este projeto está licenciado sob a [Licença MIT](LICENSE).

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
| Tarefas bloqueadas por terceiros | Logs de pendência com motivo, nome da contraparte e rastreamento de resolução |
| Notas desestruturadas espalhadas em arquivos | Notas centralizadas com título, conteúdo e status (Rascunho/Publicado/Arquivado) |
| Documentos anexos misturados com notas | Upload de arquivos (PDF, DOC, DOCX) anexados diretamente às notas |
| Portabilidade de dados entre máquinas | Exportação PDF com JSON embutido + relatórios visuais para impressão |
| Uso em múltiplos sistemas operacionais | Executável auto-contido único — funciona no Windows e Linux |
| Múltiplos usuários na mesma máquina | Contas por usuário com hash BCrypt, pergunta de segurança para recuperação |
| Ferramentas complexas para fluxos simples | Interface limpa baseada em cards, ações com um clique, calendários nativos do navegador |

### Pilha Tecnológica

| Camada | Tecnologia |
|---|---|
| Linguagem | C# 12 / .NET 8 LTS |
| Framework | Blazor Server (interativo via SignalR) |
| Banco de dados | SQLite (EF Core 8) |
| Biblioteca UI | MudBlazor 8.5 |
| Geração de PDF | QuestPDF |
| Hash de senhas | BCrypt.Net-Next |
| Mapeamento de objetos | AutoMapper |
| Testes | xUnit + Moq + FluentAssertions |

### Arquitetura — Clean Architecture

```
Presentation (Blazor Server)
    ↓ depende de
Application (Casos de uso, DTOs, Interfaces de serviço)
    ↓ depende de
Domain (Entidades, Enums, Interfaces de repositório)
    ↑ implementa
Infrastructure (EF Core DbContext, Repositórios, Serviços PDF)
```

| Projeto | Responsabilidade |
|---|---|
| `TaskFlow.Domain` | Entidades (`User`, `TaskItem`, `PendingLog`, `Note`, `Attachment`, `VisualPdfRecord`), Enums, Interfaces de repositório |
| `TaskFlow.Application` | DTOs, Interfaces de serviço (`IAuthService`, `ITaskService`, `INoteService`, `IExportService`, `IImportService`), Implementações, AutoMapper |
| `TaskFlow.Infrastructure` | `AppDbContext` (SQLite), Implementações de repositório, `FileStorageService`, Geradores QuestPDF, Configuração DI |
| `TaskFlow.Presentation` | Páginas Blazor Server, Layout MudBlazor, Interop JS, Sistema de temas |

### Funcionalidades

- **Identidade**: Cadastro, login, recuperação de senha via pergunta de segurança, edição de perfil com foto
- **TaskFlow (Tarefas)**:
  - CRUD com título, descrição, data de término
  - Fluxo de status: Aberta → Pendente → Concluída (com reabertura)
  - Logs de pendência com motivo e contraparte
  - Remover pendência (reabre a tarefa)
  - Filtro por status
  - Avisos de atraso nos cards e no Dashboard
  - Dashboard com estatísticas, tarefas atrasadas, prazos da semana, distribuição de status
- **NoteManager (Notas)**:
  - CRUD com título, conteúdo, status (Rascunho/Publicado/Arquivado)
  - Sistema de anexos por upload (PDF, DOC, DOCX)
  - Listagem de anexos com formatação de tamanho
- **Exportação**:
  - **Tipo 1 — PDF de Dados**: JSON embutido para transferência/backup entre máquinas
  - **Tipo 2 — PDF Visual**: tabelas formatadas e conteúdo completo das notas para impressão
  - Seleção de escopo (Apenas Tarefas, Apenas Notas, ou Sistema Completo)
- **Importação**:
  - **Modo A — Importação de dados**: restaurar de PDFs Tipo 1
    - Estratégias de conflito: Substituir, Mesclar, Anexar
    - Filtro por data para importação seletiva
    - Detecção de propriedade externa com opções Integrar/Visualizar/Cancelar
  - **Modo B — Importação visual**: armazenar qualquer PDF para leitura offline
- **Dashboard**: Contadores de tarefas (Abertas/Pendentes/Concluídas), total de notas, lista de atrasadas, prazos da semana, barras de distribuição, tarefas e notas recentes
- **Modo Escuro**: Tema escuro em tons de cinza via CSS nativo com persistência em localStorage
- **Instância Única**: Mutex global impede execução duplicada do app

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

# Abrir no navegador
# http://localhost:5000
```

### Atalhos na Tela Inicial

#### Linux

Crie um arquivo `.desktop` na sua área de trabalho:

```bash
cat > ~/Desktop/taskflow-notemanager.desktop << 'EOF'
[Desktop Entry]
Type=Application
Name=TaskFlow NoteManager
Comment=Gerenciador de tarefas e notas offline
Exec=/home/$USER/Projetos/gerenciador_de_tarefas_e_notas/deploy/launcher/run-taskflow.sh
Icon=text-editor
Terminal=false
Categories=Office;Utility;
Keywords=task;note;manager;pdf;signature;offline;
StartupWMClass=taskflow
EOF

chmod +x ~/Desktop/taskflow-notemanager.desktop
```

> Ajuste `Exec=` para apontar para `deploy/launcher/run-taskflow.sh` dentro do seu diretório do projeto.
> Clique com o botão direito no ícone e selecione **"Permitir Inicialização"** se solicitado.

#### Windows

Crie um atalho para `deploy\launcher\TaskFlow_Launcher.bat`:

1. Clique com botão direito na Área de Trabalho → **Novo → Atalho**
2. Navegue até `C:\Users\%USERNAME%\...\deploy\launcher\TaskFlow_Launcher.bat`
3. Nomeie como `TaskFlow NoteManager`
4. Clique direito no atalho → **Propriedades → Alterar Ícone** (opcional)

Ou via linha de comando (PowerShell):

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
