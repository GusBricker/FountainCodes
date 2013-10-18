FountainCodes
=============

C# Luby Transform library with a bench mark example application.


Details
=======

Luby Transform's are a way of encoding data to gain extra redundancy against lost packets by only having to transmit a small amount of extra data.

The algorithm splits data into blocks, then randomly decides to xor a degree of those blocks together to make droplets. Those droplets would then be transferred across some medium, along with details about what blocks where used to build them up.

The receiving end, decodes droplets by using the xor operation as well. Obviously for this to work, some droplets must be recieved with a degree of 1. Managing the degree of droplets is done by the distribution chosen and it is of huge importance on the efficiency of the entire system.


Distributions
=============

The distribution used is called a Robust Soliton distribution. Ideally it will first release degree's of 1, until close to the end of transmission it will start to ramp up the degree. The ramp up point and amplitude is dependant on how the Solition distribution is initialized. 
