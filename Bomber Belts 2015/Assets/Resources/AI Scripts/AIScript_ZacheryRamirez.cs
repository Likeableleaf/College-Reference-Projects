using UnityEngine;
using System.Collections;

public class AIScript_ZacheryRamirez : MonoBehaviour
{

    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;
    public float[] bombDistance = new float[8];
    public float opponentPosition;
    public float characterPosition;
    public bool oppEscape = false;


    // Use this for initialization
    void Start()
    {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();

        playerSpeed = mainScript.getPlayerSpeed();

    }

    // Update is called once per frame
    void Update()
    {

        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        opponentPosition = mainScript.getOpponentLocation();
        characterPosition = mainScript.getCharacterLocation();

        //Your AI code goes here

        bombDistance = mainScript.getBombDistances();

        float minDistance = 1000;//minimal distance required for the next button to be moved towards and pressed
        float currentDistance; //the distance calculated from the next best choice for button press
        int bestChoice = 0;//heuristic for the best button to press in order to be near opponent and when blocking attacks of opponent
        
        int currentLocation = 0;// the current location of character in integer

        //calculate the currentlocation of character as a integer
        for (int i = 0; i < 8 ; i++)
        {
            if(i == (int)Mathf.Abs(characterPosition))//using characterPosition float and convert to int
            {
                currentLocation = i;//save currentLocation in int
            }
        }



        //get close to the opponent
        
            currentDistance = Mathf.Abs(opponentPosition - characterPosition);//the current distance to the opponent
        print(currentDistance);
            if ((currentDistance) < minDistance) // check if the current distance is less than the minimal distance to travel
            {
                
                bestChoice = (int)Mathf.Abs(opponentPosition); //best heuristic is saved for this choice of movement
                //the distance between player and opponent saved. if the opponent runs out of this distance, the character will stop chase and juggle belts to gain points
                minDistance = currentDistance;
            }
            else
            {
            oppEscape = true;
            }
            
        



        //check if near opponent so that character can block opponent's AI from using belts character is now on
        if (Mathf.Abs(opponentPosition - characterPosition) <= 2 || oppEscape )
        {
            for (int i = 0; i < 8; i++)
            {//checking each row of buttons
                //calculate the current botton closest to character
                currentDistance = Mathf.Abs(buttonLocations[i] - characterPosition);//the current distance to the next best button press
                if (beltDirections[i] <= 0 && buttonCooldowns[i] <= 0) //check if its cooldown is down
                {
                    if (currentDistance <= minDistance)//if distance is closed in on closes button when cool down is down
                    {
                        bestChoice = i; //save the current position when on button to press
                        minDistance = currentDistance;//save the next best minimal distance to button press
                    }
                }


            }
        }

      
       
        
            if (buttonLocations[bestChoice] > characterPosition)//if the button location is above player position move up, if below, move down.
            {
                //move up and press the button closest

                mainScript.moveUp();
                mainScript.push();
            }
            else
            {
                //move down and press the button closest

                mainScript.moveDown();
                mainScript.push();
            }

        


    }
}

