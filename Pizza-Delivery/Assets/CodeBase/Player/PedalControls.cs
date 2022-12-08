// Writen by Boris Chuprin smokerr@mail.ru

using System.Collections;
using UnityEngine;

namespace CodeBase.Player
{
    public class PedalControls : MonoBehaviour
    {

        private Bicycle linkToBike;// making a link to corresponding bike's script

        public GameObject pedalLeft;
        public GameObject pedalRight;

        private GameObject ctrlHub;// gameobject with script control variables 
        private controlHub outsideControls;// making a link to corresponding bike's script

        private float energy = 0;//energy of pedaling to release after acceleration off

        //we need it to move CoM left and right to immitate rotation of pedals
        public Transform CoM; //CoM object

        //BIKE for stunts
        public Transform stuntBike;

        //special temp status "on stunt" to prevent fall after maximus angle exceed
        public bool stuntIsOn = false;

        //tmp "true" during "in stunt" 
        private bool inStunt = false;
        void Start()
        {

            ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
            outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

            linkToBike = GameObject.Find("Player").GetComponent<Bicycle>();

        }

        void FixedUpdate()
        {
            //pedals rotation part
            if (outsideControls.Vertical > 0)
            {
                this.transform.rotation = this.transform.rotation * Quaternion.Euler(linkToBike.bikeSpeed / 4, 0, 0);
                pedalRight.transform.rotation = pedalRight.transform.rotation * Quaternion.Euler(-linkToBike.bikeSpeed / -4, 0, 0);
                pedalLeft.transform.rotation = pedalLeft.transform.rotation * Quaternion.Euler(-linkToBike.bikeSpeed / 4, 0, 0);
                if (energy < 10)
                {
                    energy = energy + 0.01f;
                }

                var tmpCoM01 = CoM.transform.localPosition;
                tmpCoM01.x = -0.02f + (Mathf.Abs(this.transform.localRotation.x) / 25);//leaning bicycle when pedaling
                CoM.transform.localPosition = tmpCoM01;

            }
            else EnergyWaste();//need to move pedals some time after stop acceleration

        }

        //function when player stop accelerating and rider still slowly rotating pedals
        void EnergyWaste()
        {
            if (energy > 0)
            {
                var tmpEnergy = 10 - energy;
                this.transform.rotation = this.transform.rotation * Quaternion.Euler((linkToBike.bikeSpeed - tmpEnergy) / 4, 0, 0);
                pedalRight.transform.rotation = pedalRight.transform.rotation * Quaternion.Euler(-(linkToBike.bikeSpeed - tmpEnergy) / -4, 0, 0);
                pedalLeft.transform.rotation = pedalLeft.transform.rotation * Quaternion.Euler(-(linkToBike.bikeSpeed - tmpEnergy) / 4, 0, 0);
                energy = energy - 0.1f;

            }
        }

        //trick to do not crash for one second. You need that for riding ramps
        IEnumerator StuntHoldForOneSecond()
        {
            stuntIsOn = true;
            yield return new WaitForSeconds(0.5f);//1 second seems too long :) now it's half of second 0.5ff. Make 1 for actually 1 second
            if (!inStunt)
            {
                stuntIsOn = false;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // applying physical forces to immitate stunts///////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //void StuntBunnyHope (){
        IEnumerator StuntBunnyHope()
        {
            stuntBike.GetComponent<Rigidbody>().AddForce(Vector3.up * 40000);//push bike up
            yield return new WaitForSeconds(0.1f);//a little pause between applying force
            stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -14000);//pull front wheel(turn bike around CoM)
            yield return new WaitForSeconds(0.2f);//a little pause between applying force
            stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * 20000);//push front down and pull rear up
        }
    }
}