## Development Environment
Unity version 2019.4.21f1 (LTS)


## Milestones

1. Milestone (week 4): Requirement analysis and planning: user stories, use case diagram, class diagram, mockups about how the game will look like.
2. Milestone (week 7): 30% of features are ready, the project is version controlled, documentation in the code.
3. Milestone (week 10): 90% of features are ready, the project is version controlled, documentation in the code, CI is used, there are some unit tests.
4. Milestone (week 13): 100% of features are ready, the project is version controlled, documentation in the code, CI is used, comprehensive unit tests, clean code.

# The Game: Amusement Park

## Introduction
Design and implement a real-time theme park simulator (time can be sped up).


The player has a rectangular area where she can build their own amusement park by placing buildings. The buildings can be different rides and attractions (games), and restaurants. The player can also build gardens and paths. She receives some initial funds to build a park at the start of the game. The park starts to receive guests when the player opens it. The guests have to pay an entry fee to get into the park. They can use the games, and buy food and drinks from the restaurants. The guests pay for these, and the prices are determined by the player. Some of the games may be covered by the entry fee.

## Games
We call both the rides and attractions “games”. Building games requires time and money. They can be built only on an empty area. Operating the games costs money, which is substracted from the money of the player each time the game is used.

The games can break down. When this happens, they have to be repaired by a repairman, which also costs money. The game cannot be used by the guests while it is repaired.

The games have a capacity (how many guests can use them at once), and a turn time (how long does one turn take on the game). The guests above capacity (the guests for whom there is no room on the ride yet) are waiting near the ride.

To summarize, a game can be in the following states: in use, waiting for guests, under repair, being built.

## Gardens
We can plant trees, shrubs, or grass on empty areas anywhere in the park. These costs money, but improve the mood of the guests.

## Restaurants
Guests get hungry or thirsty from time to time, so it is important for the park to have buffets, sweet shops, hotdog stands, etc. The guests have to pay for food and drinks of course.

## Paths
Paths need to be built in the park. The guests can only get to places that are reachable from the entrance of the park. Building the paths costs money per tile.

## Guests
Guests like to periodically use the games or eat, drink. They choose the game or restaurant randomly. Their mood will diminish if they have to wait in line a lot at the game or restaurant of their choice. They will leave the park if their mood tanks (gets to zero).

Guests can only move around via the paths (they cannot step off the paths).

The mood of the guests is improved if they use a game or eat, drink. The willingness of the guests to use a game or buy something depends on the cost. We can assume that the guests have unlimited money in the context of the amusement park (we don’t have to keep a record of their money). Guests are prone to throw away the packaging of the food and drinks while walking. If they can find a thrashcan in the vicinity then they will go to it and throw the thrash into it. Otherwise they will just throw it away onto the path. Thrash on the paths decreases the mood of the guests.

## Staff
There are two kind of staff:

- Cleaner: They keep the park clean by removing thrash from the paths. They move around in their designated area. More of them can be employed and they can also be moved around if they are needed elsewhere.

- Repairman: They repair the games that are broken. Repairing a game costs time and money. They always go to and repair the game that has broken down the earliest.

## Tasks for Building the Game
- [ ] Doing the view part of the game, implementing building.
- [ ] Doing the model part which contains the games, the restaurants, plants, paths, and the staff.
- [ ] Implementing pathfinding – the vertices of the graph will be the games, restaurants, crossroads of the paths, and the entrance of the park.
- [ ] Implementing the logic of the game: the behaviour of the guests and staff and the logic of their interactions.

## Advice
- Although the game is real-time, time of course flies in the simulation faster than in the real world.

- In the prototype phase, the things in the game can be shown with temporary pictures. They should be changed to at least symbolic images in the final product.

- The characters and the games don’t have to be animated. The games should show their status (in use, waiting, under repair, being built). The characters should move along the paths.

## Extension for teams with 4 members
- The guests no longer have infinite money. The player can build ATMs where they can get more money (they still have infinite money in their account).

- There are thieves in the park! They have a thieves den in the sewers below the park. Its entrance is somewhere on a path, preferably at a crossroads far from the entrance of the park. The thieves target the guests and try to steal their money. Each thief has a skill level between 0 and 100. If a random number between 0 and 100 is less than the skill level, the thief will successfully steal the money. Otherwise the guest will call security and the thief will have to run.

- There is a new building for the security forces of the park. There is also a new staff type called security which roams the paths of the park. If a guest calls security then all the security in the park will try to catch the thief. The thief is faster than the security and he will try to avoid security and get to the thieves den before beign caught. If a thief is caught he will be escorted to the security building, then the police will be called. The police will enter the park through the entrance, get to the security building, then escort the thief out of the park.

