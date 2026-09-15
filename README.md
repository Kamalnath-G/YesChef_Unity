Yes Chef!

A 3D single-kitchen cooking game built in Unity 6 (URP) and C# for the Tentworks Interactive developer test.

The player runs a one-person kitchen for a 3-minute session: pull raw ingredients from the refrigerator, prepare them at the correct station (chop vegetables, cook meat, serve cheese as-is), and deliver them to whichever of the four customer windows requested them. Order score is the sum of ingredient values minus elapsed order time, so slow orders can score negative.

Running the Project:
Open the project in Unity 6000.4.12f1 (or any Unity 6000+).
Open Assets/Scenes/Main.unity and enter Play Mode, or run the executable in the provided Windows build folder.
Press Start on the main menu. WASD to move, E to interact.

Architecture:
The project is built around small components with single responsibilities, communicating through events and interfaces rather than direct references, so that systems can be added or replaced without editing existing ones.

Interaction:
PlayerController casts a forward ray and calls into anything implementing IInteractable. It has no knowledge of what a fridge, stove, or customer window actually is — it only knows something interactable is in front of it.

This is the main extensibility point in the project. Adding a new station (a sink, a serving counter, a second fridge) means writing one class that implements the interface and placing it in the scene. No changes to the player, no growing switch statement, no new dependencies pointing back at gameplay code.

Global State and Events:
GameManager owns the things that are genuinely global — game timer, total score, session lifecycle — and broadcasts changes:

OnGameStarted
OnGameOver
OnGameTimeChanged
OnScoreChanged

UIController subscribes to these and is the only class that touches UI. Gameplay systems never reach into UI directly. This keeps the presentation layer swappable (uGUI today, UI Toolkit later) and prevents the common failure mode where score and timer logic slowly leak into UI scripts.

Orders:
OrderManager handles generation and lifecycle — rolling 2- or 3-ingredient orders, assigning them to windows, and the 5-second respawn delay. CustomerWindowHandler owns everything about a single window: its current order, its own timer, delivery validation, per-order scoring, and score feedback.
The split matters because each window's timer runs independently. Centralising per-window timing in OrderManager would mean one class tracking four parallel timers and four sets of remaining-ingredient state — the kind of thing that works with four windows and breaks at eight.

Stove:
For the same reason, CookingSlot is a separate class from Stove. Each slot tracks its own coroutine, its own 6-second cook, and its own progress UI. Stove just holds a collection of slots and routes an incoming meat to the first free one.

Adding a third slot is a prefab change, not a code change.

Reset:
GameManager coordinates restarts, but each system resets itself — Stove stops its slot coroutines, Table clears its in-progress chop, each CustomerWindowHandler drops its order. GameManager doesn't need to know what "clean" means for every system, only that it should ask them all.

Time.timeScale is restored on restart so a restart from the pause menu resumes correctly.

Design Decisions:
The spec left several interaction details open. These are the calls I made:

Raycast interaction over trigger volumes. A forward raycast means the player must face a station to use it, which makes interaction unambiguous when the fridge and a customer window are close together. Trigger volumes would have been simpler but produce "which one did I just interact with?" moments near corners.

Fridge opens a selection UI. The alternative was three separate pickup points on the refrigerator. A selection panel keeps the fridge a single interactable and scales cleanly if the ingredient list grows past three.

Unwanted ingredients stay in hand. Per the spec, delivering an ingredient the current order doesn't need leaves it in hand rather than consuming it. The trash exists as the deliberate discard path, so a mistake costs time but isn't unrecoverable.

Separate representations for gameplay and UI. IngredientType is the shared identifier; 3D prefabs represent ingredients in the world, dedicated sprites represent them in order UI. Order UI therefore doesn't depend on prefab setup.

Known Limitations / Next Steps:

Scope calls made to keep this within the test's time budget:

IngredientType is an enum. For a real game I'd move ingredients to ScriptableObjects holding score value, prep station, prefab, and icon. Designers could then add ingredients without a code change, and the prep-routing logic would become data rather than a switch.
Ingredient prefabs are instantiated and destroyed per pickup. Fine at this volume; would want pooling before adding particles or a longer session.
No audio. The spec explicitly excluded it.
UI is uGUI with fixed anchoring, tested at 16:9. Extreme aspect ratios aren't handled.