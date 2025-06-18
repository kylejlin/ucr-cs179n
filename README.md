# Trilobyte Project Report

### Video

Youtube video link: [Trilobyte- CS 179N Senior Project 
](https://www.youtube.com/watch?v=UskI1BW1ZgI)
## Overview

Trilobyte is a 3D single-player aquarium simulator built in Unity. The player manages an aquarium full of prehistoric creatures. They must maintain a balanced ecosystem, so that no species will die off. Players can also buy decorations, which increase organisms’ happiness, leading to faster reproduction and ecosystem growth. 

## Game Description 
Trilobyte is a 3D single-player aquarium simulator, where the goal of the game is to build an aquarium, populate it with creatures and decorations, and manage the ecosystem inside the tank. Many of the creatures you can get in the game are based on extinct fauna from millions of years ago that we know only from the fossil record. The game has a somewhat sandbox format: there is progression based on how much money the player has earned so far, but the player has plenty of room to do whatever they please with the money they have and observe the results. 

The player starts with nothing in the tank and little money, but they can use the money to buy an algae. The algae grows and reproduces, spreading across the tank, and as the tank fills the player earns a bit more money. Once they have enough money, they can buy the next rung up the food chain: a trilobite. The trilobites consume the algae, keeping its population in check and reproducing once they grow large enough. The cycle repeats, allowing the player to buy the carnivore anomalocaris, to keep the trilobites in check. Finally, the player can buy multiple tanks to tend and grow even further. 

The amount of money earned is based on both the number of creatures total as well as their happiness. Their happiness is based on how much food they can eat, how many other members of their species are present, and how many decorations are in their tank. The most productive tank would be one with a dense and stable ecosystem with many decorations. Anomalocaris produce the most money, but they require the rest of the ecosystem to survive.

Each creature is independent, making its own choices based on its internal state machine, and can interact in a complex way with everything else in the ecosystem. When it is hungry, it will eat, and when it is healthy, it will breed with another of its own species. Whenever a creature eats, it gets a certain amount of energy that it takes from whatever it eats, meaning there is a finite amount of energy in the system that gets transferred whenever eating occurs. To have a stable ecosystem, the player must ensure that each level of the food chain is stable- there can’t be too many herbivores or the plants will all die out, and there can’t be too many carnivores or the herbivores will all die out. This leads to emergent behavior, where based on simple rules, complex situations arise. 

To help the player, there are certain UI/UX features that we implemented. When a player clicks on a creature, it will show its energy and other stats so the player understands better how the creature is doing. A player can also go to the tank tab to see a list of everything in the tank and check on specific individuals. There is also flavor text in the shop to let the player know where in the food chain each creature is. 

## Implementation 

**Game Engine:** Unity

**Scene Structure:** The game has two scenes: the main menu and the gameplay. The main menu has credits, options, and a button that transitions the game to the main gameplay scene, which contains UI objects, the camera, and the Game Manager that sets things up. 

**Class Structure:**

- **UI:** BaseUI class is used for most UI elements. Classes for shop items, the tank menu, and others inherit from it.

- **Game Manager:** Holds a list of all the aquariums in the game. Takes care of any functions that require searching / calling all of the aquariums.
  
- **Aquarium:** Holds a list of all entities within it. Takes care of all functions that need to check everything in the tank such as adding and removing entities. Also contains and builds the pathfinding voxel array.
  
- **Entity:** Base Entity class is used for any object that can be bought, moved, deleted, and put in the tank. Creature and Decoration classes inherit from it. Mobile Creature and Immobile Creature inherit from Creature. Finally, all creatures in the game have their own class that inherits from Mobile Creature or Immobile Creature. This is where the internal logic of the creatures is written.
  
- **Creatures:** Each creature has a prefab where they are given a model, audio, animations etc. Internally, they use a state machine to decide to eat, wander, or breed according to their energy and maturity. They also have a happiness value based on their energy and the number of other creatures and decor.

**Pathfinding:** Predators calculate path towards prey based on gradient ascent. The world is partitioned into voxels (cubic regions), forming a three dimensional grid. Each voxel has a scent value, set by a flood fill from all food sources (maximum value for all voxels intersecting a food source bounding box). Voxels occupied by non-food entities (e.g., rocks) block the propagation of the scent flood. Creatures move towards the nearby voxels with the highest value to find the food. 

**Game UI:** There are three canvases in the game: Shop, Callery, and Tank. The shop allows players to buy creatures and decorations using in-game currency. It has categories for different types of objects or creatures and displays their cost and information about the item. The gallery will enable players to view the entities they have collected. Tank UI allows players to switch between different tanks and buy new tanks.

**Graphics:** The graphics style was low poly and low resolution, inspired by 3D PS1 games and early PC games. Low resolution was accomplished using a post processing effect over the whole game. All textures were imported with no compression or filters so that the pixels were crisp. All models in the game used the unlit shading model, so the colors were bright and flat, with no shadows except those that were manually drawn in the model textures.
  
**3D Models:** The original 3D models were made in Blender. Kyle made the algae, and Amelia made the trilobite, anomalocaris, some of the decorations, their textures, and animations. They were imported into Unity with their animations but without their textures, which were applied using materials.

All other models, music, textures and fonts are attributed within the game inside the credits menu. 


