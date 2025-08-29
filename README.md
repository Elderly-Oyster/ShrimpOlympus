# ShrimpOlympus

Technical Unity repository with modular architecture for developing and testing various mini-projects.

## 🏗️ Architecture

The project is built on modular architecture principles, where each module is an isolated and independent component. The architecture follows the **MVP (Model-View-Presenter)** pattern with the possibility of using **Stateless** for state management.

### Core Principles:
- **Modularity**: Each module is encapsulated and independent
- **MVP Pattern**: Separation of logic into Model, View, and Presenter
- **Dependency Injection**: Using VContainer for dependency management
- **Asynchrony**: UniTask support for asynchronous operations

## 🛠️ Development Tools

### Module Creator
Built-in tool for automatically creating new modules:
- **Access**: `Tools > Create Module` in Unity Editor
- **Capabilities**:
  - Automatic folder structure creation
  - Generation of basic scripts (Installer, Presenter, View, Model)
  - Creation of Assembly Definition files
  - Generation of scenes and prefabs
  - Module type selection (Base, Additional, Test)

### Module Types:
- **Base**: Core application modules
- **Additional**: Additional modules
- **Test**: Test modules for debugging

## 📦 Existing Modules

### Base Modules
- **TicTac**: Mini-game "Tic-tac-toe" with full MVP implementation
- **Converter**: Data conversion utility
- **MainMenu**: Main application menu
- **StartGame**: Game launch screen

### Test Modules
- **PopupsTester**: Test module for demonstrating the popup system
  - Implements PopupHub for modal window management
  - Contains test buttons for various scenarios
  - Demonstrates work with R3 (Reactive Extensions)

## 🏛️ Project Structure

```
Assets/
├── CodeBase/                 # Main codebase
│   ├── Core/                # System core
│   │   ├── Patterns/        # Architectural patterns
│   │   │   └── MVP/         # MVP interfaces
│   │   ├── Systems/         # System components
│   │   │   └── PopupHub/    # Popup management system
│   │   └── UI/              # UI components
│   ├── Services/            # Application services
│   ├── Editor/              # Editor tools
│   │   └── ModuleCreator/   # Module creator
│   └── Tests/               # Tests
├── Modules/                  # Application modules
│   ├── Base/                # Base modules
│   ├── Additional/          # Additional modules
│   └── Test/                # Test modules
└── Resources/                # Resources
```

## 🚀 Quick Start

1. **Clone the repository**
2. **Open the project in Unity** (recommended Unity 2022.3 LTS or newer)
3. **Create a new module**:
   - In Unity Editor: `Tools > Create Module`
   - Choose module name
   - Select module type
   - Configure components to create
   - Click "Create Module"

## 📋 Requirements

- Unity 2022.3 LTS or newer
- .NET 4.x
- Supported platforms: Windows, macOS, Linux

## 🔧 Technologies

- **Unity**: Main engine
- **VContainer**: Dependency Injection container
- **UniTask**: Asynchronous operations
- **R3**: Reactive Extensions for Unity
- **Stateless**: State management (optional)

## 📝 Features

- **Not a game**: This is a technical repository for development and testing
- **Mini-projects**: Each module represents a separate mini-project
- **Popup system**: Built-in modal window management system
- **Testing**: Support for creating test modules for debugging

## 🤝 Contributing

1. Fork the repository
2. Create a branch for new feature
3. Make changes
4. Create Pull Request

## 📄 License

[Specify project license]

---

**ShrimpOlympus** - technical platform for modular development in Unity.
