# CS 426 - Assignment 6

## AI and Mecanim; Physics, Lights, Textures

### Team Members

- Peter Pacholarz
- Dylen Greenenwald
- Hossein Fatho

### Design

Our level design is a very rough prototype. However, there are seeds to some of the more complex and innovative ideas that we wish to implement. These deliverables will be more present in the Alpha release; unfortunately, we had some collaborative issues that prevented a more robust implementation for this assignment.

The main idea behind this simple introductory level is to allow players to get comfortable with navigating the maze, given that the controls demand potentially simultaneous keyboard and mouse usage. The paths are not obstructed, traps (i.e. that teleport you) are not yet implemented; this all makes for an extremely usable floor plan, but one that is not very challenging. This is because we want to include a *real* level that the players will experience that allows them to learn the game without an explicit tutorial and also without getting to frustrated to advance further. We intend on incorporating dialogue in this simple level for illustrative purposes -- enemies die immediately upon contacting the player at this point in time.

The theme of the game is basically that we are inside of a process, and want to transport a malicious payload to a critical location in order to wreak some sort of havoc (it is a work in progress). This manifests in boxes representing these payloads, and our character having to navigate antivirus and other security mechanisms to complete the exploitation.

Two types of enemies are defined:
The first enemy moves along the table paths, patrolling, and encountering it deducts points from the player.
The second enemy consists of spirits that move randomly and collectively based on flocking behavior, capable of moving among walls deducting a very small amount of points from the player upon collision.

### Division of Labor

#### Artificial Intelligence

- AI construct 1 (Peter):
  - Pathfinding
    - Leverage Unity AI's NavMeshes and pathfinding to give the initial enemy default behavior
- AI construct 2 (Hossein):
  - Flocking
    1. Create an empty object to instantiate the flocking behavior.
    2. Write a flocking script that defines the ghosts' speed and movement type and assigns it to the empty object, then attach the ghost prefab.
    3. Write another script to specify each flocking member's movement and speed, define their collision boundaries, and assign it to the ghost prefab.
    4. Define movement ranges, speed values, and directions for each member within specific intervals incorporate them into the flocking manager script and finally assign them to the empty object. Then, place this object in the game scene during gameplay for symmetry.
- AI construct 3 (Dylen):
  - Finite State Machines
    - Constructs a modular FSM codebase using scriptable objects that can be extended to many states and complex systems
    - [Reference to tutorial by Garegin Tadevosyan](https://www.toptal.com/unity-unity3d/unity-ai-development-finite-state-machine-tutorial)

#### Mecanim

- Mecanim construct 1 (Peter):
  - Death animations for pathfinding AI bots
  - Walking animation for player
- Mecanim construct 2 (Hossein):
  - Movement animations for unused characters
    - Miscommunication caused these to be artifacts, but there were fully functional walking, running, and stopping animations
    - "I have designed a master enemy for our game that resembles a demon and has the ability to chase, run, and grab.
      When a player approaches it closely, they can grab and pull it, or pursue it within a specific interval. 
      It also has the ability to roam within a defined range in the game."
- Mecanim construct 3 (Dylen):
  - Walking and running animations for FSM-controlled AI bots