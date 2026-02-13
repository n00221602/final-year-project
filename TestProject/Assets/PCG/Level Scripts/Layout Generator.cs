using UnityEngine;

public class LayoutGen : MonoBehaviour
{
    //int[,] is a 2D array that uses rows and columns.

    //LAYOUT REMINDER:
    //Y=0{ 1   ,2   ,2} 
    //Y=1{ 2   ,3   ,3} 
    //Y=2{ 2   ,3   ,3}
    //    X=0  X=1  X=2

    //LEGEND
    //0 = empty space
    //1 = corner
    //2 = wall
    //3 = floor
    //4 = door/entry point
    //5 = exit point

    //TEST LAYOUTS 

    //Basic sqaure
    // { 1,2,2,2,1},  
    // { 2,3,3,3,2},
    // { 2,3,3,3,2},
    // { 2,3,3,3,2},
    // { 1,2,2,2,1}

    //L-Shape
    // { 1,2,2,2,2,2,2,2,2,2,1},
    // { 2,3,3,3,3,3,3,3,3,3,2},
    // { 2,3,3,3,3,3,3,3,3,3,2},
    // { 1,2,2,2,2,2,1,3,3,3,2},
    // { 0,0,0,0,0,0,2,3,3,3,2},
    // { 0,0,0,0,0,0,2,3,3,3,2},
    // { 0,0,0,0,0,0,1,2,2,2,1}

    //V-Shape (Co-Pilot Generated + tweaked)
    // { 1,2,2,2,2,2,1,0,1,2,2,2,2,2,1},
    // { 2,3,3,3,3,3,2,0,2,3,3,3,3,3,2},
    // { 2,3,3,3,3,3,2,0,2,3,3,3,3,3,2},
    // { 2,3,3,3,3,3,2,0,2,3,3,3,3,3,2},
    // { 2,3,3,3,3,3,2,0,2,3,3,3,3,3,2},
    // { 2,3,3,3,3,3,2,0,2,3,3,3,3,3,2},
    // { 1,1,3,3,3,3,2,0,2,3,3,3,3,1,1},
    // { 0,1,1,3,3,3,2,0,2,3,3,3,1,1,0},
    // { 0,0,2,3,3,3,2,0,2,3,3,3,2,0,0},
    // { 0,0,1,1,3,3,2,0,2,3,3,1,1,0,0},
    // { 0,0,0,2,3,3,1,2,1,3,3,2,0,0,0},
    // { 0,0,0,1,1,3,3,3,3,3,1,1,0,0,0},
    // { 0,0,0,0,2,3,3,3,3,3,2,0,0,0,0},
    // { 0,0,0,0,1,2,2,2,2,2,1,0,0,0,0},
    // { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}

    public int[,] layout1 = new int[,]
    {
     { 1,2,2,2,1},
     { 2,3,3,3,2},
     { 4,3,3,3,2},
     { 2,3,3,3,2},
     { 1,2,2,2,1}
    };

    public int[,] layout2 = new int[,]
    {
     { 1,2,2,2,2,2,2,2,2,2,1},
     { 2,3,3,3,3,3,3,3,3,3,2},
     { 2,3,3,3,3,3,3,3,3,3,5},
     { 1,2,2,2,2,2,1,3,3,3,2},
     { 0,0,0,0,0,0,2,3,3,3,2},
     { 0,0,0,0,0,0,2,3,3,3,2},
     { 0,0,0,0,0,0,1,2,2,2,1}
    };

    //NOTE: Should wall and corner prefabs just have a full floor tile? Opposed to having floors only on the "inner" sections?
    //If not then i will have to create an "inner corner" prefab, so having each prefab contain its own floor tile would be easier but
    //it would also include pertruding floor tiles under walls and corners. Also it's rendering unseen floor tiles which might cause avoidable performance problems.
    //Make another prefab, but not another number.Adding too many numbers will make the AI training more complicated.
    //Maybe add numbers later for added room complexity? Or just add complexity through the room design script.


    //Layout note:
    //Each "level" consists of multiple rooms linked together. A script links these together, while the AI creates the room layout itself. A level is a floor in the facility.
    //All levels are pre-generated om loading time. Research creating "Dungeon Layouts". Look up how going under made their layouts. (another unity game)
}
