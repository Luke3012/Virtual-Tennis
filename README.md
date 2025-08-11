# Virtual Tennis+

Virtual Tennis+ is a single-player VR tennis game developed in Unity, designed to deliver an immersive and interactive sports experience.  
Its standout feature is seamless integration with a **Companion App**, allowing players to control shots and gameplay dynamics through their smartphone sensors.

---

## 🎮 Gameplay
The game offers two main modes:
- **Arcade Mode** – Choose the opponent's difficulty (Easy, Medium, Hard) and play quick matches.
- **Tournament Mode** – Face opponents of increasing difficulty, earning trophies with each win. A single defeat returns you to the main menu.

The player can:
- Control the racket via **keyboard** or **Companion App**.
- Perform strong or weak shots depending on gesture speed.
- Switch between first-person and third-person views.
- Pause and navigate menus directly from the app or keyboard.

---

## 🛠 Technical Overview
- **Architecture**: Client–server model, with the game acting as the server and the Companion App as the client.
- **Networking**:  
  - UDP broadcast (port 5001) for discovery.  
  - TCP/IP connection for continuous communication and automatic reconnection.

---

## 📂 Core Components
- `CaricaPersonaggio` – Dynamically loads chosen player/AI prefabs.
- `CaricaScena` – Handles asynchronous scene loading with camera animations.
- `Musica` (singleton) – Manages background music across scenes.
- `RacchettaManager` (singleton) – Manages app connection and command processing.
- `GestionePunteggio` – Controls scoring logic, game states, and camera events.
- `Giocatore` / `Bot` – Player and AI logic, each inheriting from `GiocatoreBase`.
- `Palla` – Governs ball movement, collisions, and point assignment.

---

## 🎯 AI Difficulty Levels
Implemented via the `IDifficolta` interface:
- **Easy**: Slow speed, high error rates.
- **Medium**: Balanced stats.
- **Hard**: High speed, low error probability.

---

## 📷 Camera System
The `CameraController` supports smooth transitions between perspectives using linear (`lerp`) and spherical (`slerp`) interpolation.

---

## 🖼 UML Diagram
![UML Diagram](images/uml-virtual-tennis+.png)

---

## 📸 Screenshots

### Main Menu
![Main Menu 1](images/vt+1.png)
![Main Menu Calibration](images/vt+2.png)

### Player & Scenario Selection
![Player Selection](images/vt+3.png)
![Scenario Selection](images/vt+4.png)

### In-Game – Third Person Mode
![Third Person Mode 1](images/vt+5.png)
![Third Person Mode 2](images/vt+10.png)

### In-Game – First Person Mode
![First Person Mode](images/vt+7.png)

### Pause Menu
![Connection Screen](images/vt+9.png)

### Scoring & UI Elements
![Score Display](images/vt+8.png)

### Victory & Trophy Screens
![Victory Bronze](images/vt+6.png)

### Other
![Other 1](images/vt+11.png)
![Other 2](images/vt+12.png)
![Other 3](images/vt+13.png)

---

## 📜 Credits
Assets and sound effects from:
- THE DFAULTS by fergicide (itch.io)  
- I2TextAnimation  
- Fontget  
- Trophy Cups/Chalices FREE (Unity Asset Store)  
- Soundsnap - Tennis Sound Effects  
- Bensound Music  
