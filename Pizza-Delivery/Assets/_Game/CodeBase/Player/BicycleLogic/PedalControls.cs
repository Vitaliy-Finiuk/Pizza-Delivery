using _Game.CodeBase.Infrastucture;
using _Game.CodeBase.Services.Input;
using UnityEngine;

namespace _Game.CodeBase.Player.BicycleLogic
{
    public class PedalControls : MonoBehaviour
    {
        private IInputService _inputService;
        
        [SerializeField] private Bicycle _linkToBike;

        public GameObject pedalLeft;
        public GameObject pedalRight;

        private float energy = 0;

        public Transform CoM; 

        public Transform stuntBike;

        public bool stuntIsOn = false;

        private bool _inStunt = false;

        private void Awake()
        {
            _inputService = Game.InputService;
        }

        private void FixedUpdate()
        {
            if (_inputService.Axis.y > 0)
            {
                PedalPart();
            }
            else EnergyWaste();

        }

        private void PedalPart()
        {
            this.transform.rotation *= Quaternion.Euler(_linkToBike.BikeSpeed / 4, 0, 0);
            pedalRight.transform.rotation *= Quaternion.Euler(-_linkToBike.BikeSpeed / -4, 0, 0);
            pedalLeft.transform.rotation *= Quaternion.Euler(-_linkToBike.BikeSpeed / 4, 0, 0);
            
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
                this.transform.rotation *= Quaternion.Euler((_linkToBike.BikeSpeed - tmpEnergy) / 4, 0, 0);
                pedalRight.transform.rotation *= Quaternion.Euler(-(_linkToBike.BikeSpeed - tmpEnergy) / -4, 0, 0);
                pedalLeft.transform.rotation *= Quaternion.Euler(-(_linkToBike.BikeSpeed - tmpEnergy) / 4, 0, 0);
                energy -= 0.1f;
            }
        }

    }
}