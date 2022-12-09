using CodeBase.Input;
using UnityEngine;

namespace CodeBase.Player
{
    public class PedalControls : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        private Bicycle linkToBike;

        public GameObject pedalLeft;
        public GameObject pedalRight;

        private float energy = 0;

        public Transform CoM; //CoM object

        //BIKE for stunts
        public Transform stuntBike;

        public bool stuntIsOn = false;

        private bool _inStunt = false;

        private void Start()
        {
            linkToBike = GameObject.Find("Player").GetComponent<Bicycle>();
            _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        }

        private void FixedUpdate()
        {
            if (_playerInput.Vertical > 0)
            {
                PedalPart();
            }
            else EnergyWaste();

        }

        private void PedalPart()
        {
            this.transform.rotation *= Quaternion.Euler(linkToBike.bikeSpeed / 4, 0, 0);
            pedalRight.transform.rotation *= Quaternion.Euler(-linkToBike.bikeSpeed / -4, 0, 0);
            pedalLeft.transform.rotation *= Quaternion.Euler(-linkToBike.bikeSpeed / 4, 0, 0);
            
            if (energy < 10) 
                energy += 0.01f;

            var tmpCoM01 = CoM.transform.localPosition;
            tmpCoM01.x = -0.02f + (Mathf.Abs(this.transform.localRotation.x) / 25);
            CoM.transform.localPosition = tmpCoM01;
        }
        
        private void EnergyWaste()
        {
            if (energy > 0)
            {
                var tmpEnergy = 10 - energy;
                this.transform.rotation *= Quaternion.Euler((linkToBike.bikeSpeed - tmpEnergy) / 4, 0, 0);
                pedalRight.transform.rotation *= Quaternion.Euler(-(linkToBike.bikeSpeed - tmpEnergy) / -4, 0, 0);
                pedalLeft.transform.rotation *= Quaternion.Euler(-(linkToBike.bikeSpeed - tmpEnergy) / 4, 0, 0);
                energy -= 0.1f;
            }
        }

    }
}