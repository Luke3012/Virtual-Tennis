VirtualTennis+ is a real-time tennis simulator built in Unity. It combines a robust scoring state machine with precise ball–racket physics and opponent AI, delivering realistic rallies and match flow.

The codebase emphasizes clean separation of concerns, event-driven updates, and testability.
• 	Core systems
• 	Scoring engine: Full tennis logic (points, games, sets, tie-break) via a deterministic state machine.
• 	Physics: Rigidbody-based ball flight, spin, bounce coefficients by surface, timed racket collisions.
• 	AI opponents: Finite state machines for serve/return/positioning, shot selection by risk profile.
• 	Input & camera: Unity Input System (player), broadcast and follow cams with smooth transitions.
• 	UI/UX: uGUI or UI Toolkit for scoreboard, serve indicators, and match timeline.
• 	Architecture
• 	Managers: MatchManager, ScoreSystem, BallController, PlayerController, AIController.
• 	Data: ScriptableObjects for player archetypes, courts, rackets; JSON for match logs and settings.
• 	Events: C# events/UnityEvents for score updates, rally outcomes, and UI sync.
• 	Platforms
• 	Desktop builds (Windows/macOS); adaptable to console or mobile with input profile swaps.

Ideal for coursework, demonstrations, and experimenting with AI/physics trade-offs.

Project report
• 	Objectives
• 	Deliver a credible tennis feel with readable code.
• 	Separate gameplay, presentation, and data for maintainability.
• 	Design overview
• 	A Scoring State Machine advances from point → game → set, including tie-break conditions.
• 	Ball Physics uses Rigidbody, lift/drag approximations for topspin/slice, and PhysicMaterials per surface.
• 	AI FSM cycles through anticipate → move → prepare → swing → recover, weighted by player style.
• 	Key systems
• 	Serve pipeline: Toss timing → contact window → fault/let checks → advantage tracking.
• 	Rally resolution: Contact point + swing vector → impulse + spin → trajectory integration → bounce response → forced error/winner evaluation.
• 	Shot selection: Risk model blends opponent position, ball height, and stamina into aim zones.
• 	UI and telemetry
• 	Live scoreboard, mini-map positions, and a rally feed.
• 	Point-by-point JSON export for post-match analysis or Companion ingestion.
• 	Performance
• 	Physics in FixedUpdate; animation and UI in Update/LateUpdate.
• 	Object reuse for particles/trails; minimal GC via pooled structs.
• 	Testing
• 	PlayMode tests for scoring edge cases; EditMode tests for state transitions.
• 	Deterministic seeds for AI/serve placement during tests.
• 	Limitations
• 	Simplified aerodynamics; no full-body IK; crowds and line calls are stubbed.
• 	Future work
• 	Online matchmaking, deeper stamina/injury model, doubles mode, replay system with scrubbing.
