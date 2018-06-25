# Evolutionary cars
This is the repository for the project of the course _sistemi intelligenti robotici (intelligent robot systems)_, University of Bologna, A.Y. 2017/2018.
## Goal of the projects
The project aims to design and implement a system in which 2D cars learn to drive on a circuit through an evolutionary robotics approach based on neural networks.
### General description
- Every car is equipped with five proximity sensors, which are placed in the front side. Each sensor provides the distance between the car and the nearest object in a certain direction.
- Every car is driven by a feedforward neural network. The net takes the five sensor values as input, and provides two value as output: the engine force (speed of the car) and direction.
- The neural net's weights make up the genotype of the car. They are initially set in a random way, and then evolved through a genetic algorithm.
- The genotype quality is given by the distance traveled by the car.
- In the evolution phase the genetic algorith performs selection, crossover and mutation to create a new generation of genotypes.
More details will be provided in the project report.
### Simulation  
We used [Godot](https://godotengine.org) and C# for simulating the car races. We created four different circuits with different difficulty levels. If you wish to run the simulation by yourself you will need Godot with C# integration. Be aware that running the simulation on the toughest circuits will need several minutes. There is no way to speed up the simulation yet.
### Examples
Easy circuit

![easy](https://github.com/manuelperuzzi/evolutionary-cars/blob/develop/examples/evo-cars_track02.gif?raw=true)

Medium-easy circuit

![medium-easy](https://github.com/manuelperuzzi/evolutionary-cars/blob/develop/examples/evo-cars_track03.gif?raw=true)

Medium-hard circuit

![medium-hard](https://github.com/manuelperuzzi/evolutionary-cars/blob/develop/examples/evo-cars_track01.gif?raw=true)

Hard circuit

![easy](https://github.com/manuelperuzzi/evolutionary-cars/blob/develop/examples/evo-cars_track04.gif?raw=true)
