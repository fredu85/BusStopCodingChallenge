# BusStopCodingChallenge
 
Made in Windows using Unity.
There is a folder called build, inside of that there is a file called index.html

Essentially I made a brute force solution where it starts at the first station,
checks all connections moves to all possible stations, making the next station the new first station,
the loop continues until it either finds the end station or runs into the same station again.

The above function returns a route if it finds the end station, several if there are multiple routes,
the shortest from those routes is then picked.
Each step of the route is compared to the buslines, assuming that the busses go from first index to last
and then back again. 

Which bus to ride is chosen based on the following:
1. Same bus as previous
2. yellow->red->green->blue
