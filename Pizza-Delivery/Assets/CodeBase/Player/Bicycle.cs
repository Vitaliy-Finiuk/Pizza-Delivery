using UnityEngine;

namespace CodeBase.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bicycle : MonoBehaviour
    {
        public WheelCollider coll_frontWheel;
        public WheelCollider coll_rearWheel;
        public GameObject meshFrontWheel;
        public GameObject meshRearWheel;
        // check isn't front wheel in air for front braking possibility
        bool isFrontWheelInAir = true;

        //////////////////////////////////////// Stifness, CoM(center of mass), crahsed /////////////////////////////////////////////////////////////
        //for stiffness counting when rear brake is on. Need that to lose real wheel's stiffness during time
        float stiffPowerGain = 0.0f;
        //for CoM moving along and across bike. Pilot's CoM.
        float tmpMassShift = 0.0f;
        // crashed status. To know when we need to desable controls because bike is too leaned.
        public bool crashed = false;
        // there is angles when bike takes status crashed(too much lean, or too much stoppie/wheelie)
        public float crashAngle01;//crashed status is on if bike have more Z(side fall) angle than this												
        public float crashAngle02;//crashed status is on if bike have less Z(side fall) angle than this 												
        public float crashAngle03;//crashed status is on if bike have more X(front fall) angle than this 												
        public float crashAngle04;//crashed status is on if bike have more X(back fall) angle than this												

        // define CoM of bike
        public Transform CoM; //CoM object
        public float normalCoM; //normalCoM is for situation when script need to return CoM in starting position										
        public float CoMWhenCrahsed; //we beed lift CoM for funny bike turning around when crahsed													

        //////////////////// "beauties" of visuals - some meshes for display visual parts of bike ////////////////////////////////////////////
        public Transform rearPendulumn; //rear pendulumn
        public Transform steeringWheel; //wheel bar
        public Transform suspensionFront_down; //lower part of front forge
        private int normalFrontSuspSpring; // we need to declare it to know what is normal front spring state is
        private int normalRearSuspSpring; // we need to declare it to know what is normal rear spring state is
        private bool forgeBlocked = true; // variable to lock front forge for front braking


        private float baseDistance; // need to know distance between wheels - base. It's for wheelie compensate(dont want wheelie for long bikes)
        
        public AnimationCurve wheelbarRestrictCurve = new AnimationCurve(new Keyframe(0f, 20f), new Keyframe(100f, 1f));//first number in Keyframe is speed, second is max wheelbar degree

        // temporary variable to restrict wheel angle according speed
        private float tempMaxWheelAngle;
        
        private Vector3 wheelCCenter;
        private RaycastHit hit;

        /////////////////////////////////////////// technical variables ///////////////////////////////////////////////////////
        public float frontBrakePower; //brake power absract - 100 is good brakes																		

        public float LegsPower; // Leg's power to wheels. Abstract it's not HP or KW or so...																	
    
        // airRes is for wind resistance to large bikes more than small ones
        public float airRes; //Air resistant 																										// 1 is neutral

        private GameObject ctrlHub;// gameobject with script control variables 
        private controlHub outsideControls;// making a link to corresponding bike's script
        /////////////////////////////////////////////////// BICYCLE CODE ///////////////////////////////////////////////////////
        private float frontWheelAPD;// usualy 0.05f
        private GameObject pedals;
        private PedalControls linkToStunt;
        private bool rearPend;

        [HideInInspector]
        public float bikeSpeed; //to know bike speed km/h
        public bool isReverseOn = false; //to turn On and Off reverse speed

        private Rigidbody _rigidbody;

        ////////////////////////////////////////////////  ON SCREEN INFO ///////////////////////////////////////////////////////
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            //if there is no pendulum linked to script in Editor, it means MTB have no rear suspension, so no movement of rear wheel(pendulum)
            rearPend = rearPendulumn;

            //bicycle code
            frontWheelAPD = coll_frontWheel.forceAppPointDistance;

            ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
            outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

            pedals = GameObject.Find("bicycle_pedals");
            linkToStunt = pedals.GetComponent<PedalControls>();

            Vector3 setInitialTensor = GetComponent<Rigidbody>().inertiaTensor;//this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);// now Center of Mass(CoM) is alligned to GameObject "CoM"
            GetComponent<Rigidbody>().inertiaTensor = setInitialTensor;////this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass

            // wheel colors for understanding of accelerate, idle, brake(white is idle status)
            meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
            meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

            //for better physics of fast moving bodies
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            // too keep LegsPower variable like "real" horse powers
            LegsPower = LegsPower * 20;

            //*30 is for good braking to keep frontBrakePower = 100 for good brakes. So, 100 is like sportsbike's Brembo
            frontBrakePower = frontBrakePower * 30;//30 is abstract but necessary for Unity5

            //tehcnical variables
            normalRearSuspSpring = (int)coll_rearWheel.suspensionSpring.spring;
            normalFrontSuspSpring = (int)coll_frontWheel.suspensionSpring.spring;

            baseDistance = coll_frontWheel.transform.localPosition.z - coll_rearWheel.transform.localPosition.z;// now we know distance between two wheels

            var tmpMeshRWh01 = meshRearWheel.transform.localPosition;
            tmpMeshRWh01.y = meshRearWheel.transform.localPosition.y - coll_rearWheel.suspensionDistance / 4;
            meshRearWheel.transform.localPosition = tmpMeshRWh01;


            //and bike's frame direction
            var tmpCollRW01 = coll_rearWheel.transform.localPosition;
            tmpCollRW01.y = coll_rearWheel.transform.localPosition.y - coll_rearWheel.transform.localPosition.y / 20;
            coll_rearWheel.transform.localPosition = tmpCollRW01;

        }

        private void FixedUpdate()
        {

            ApplyLocalPositionToVisuals(coll_frontWheel);
            ApplyLocalPositionToVisuals(coll_rearWheel);


            //////////////////////////////////// part where rear pendulum, wheelbar and wheels meshes matched to wheelsColliers and so on
            //beauty - rear pendulumn is looking at rear wheel(if you have both suspension bike)
            if (rearPend)
            {//rear pendulum moves only when bike is full suspension
                var tmp_cs1 = rearPendulumn.transform.localRotation;
                var tmp_cs2 = tmp_cs1.eulerAngles;
                tmp_cs2.x = 0 - 8 + (meshRearWheel.transform.localPosition.y * 100);
                tmp_cs1.eulerAngles = tmp_cs2;
                rearPendulumn.transform.localRotation = tmp_cs1;
            }
            //beauty - wheel bar rotating by front wheel
            var tmp_cs3 = suspensionFront_down.transform.localPosition;
            tmp_cs3.y = (meshFrontWheel.transform.localPosition.y - 0.15f);
            suspensionFront_down.transform.localPosition = tmp_cs3;
            var tmp_cs4 = meshFrontWheel.transform.localPosition;
            tmp_cs4.z = meshFrontWheel.transform.localPosition.z - (suspensionFront_down.transform.localPosition.y + 0.4f) / 5;
            meshFrontWheel.transform.localPosition = tmp_cs4;

            // debug - all wheels are white in idle(no accelerate, no brake)
            meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
            meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

            // drag and angular drag for emulate air resistance
            if (!crashed)
            {
                GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 210 * airRes; // when 250 bike can easy beat 200km/h // ~55 m/s
                GetComponent<Rigidbody>().angularDrag = 7 + GetComponent<Rigidbody>().velocity.magnitude / 20;
            }

            //determinate the bike speed in km/h
            bikeSpeed = Mathf.Round((GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10) * 0.1f; //from m/s to km/h

            ///bicycle code
            coll_frontWheel.forceAppPointDistance = frontWheelAPD - bikeSpeed / 1000;
            if (coll_frontWheel.forceAppPointDistance < 0.001f)
            {
                coll_frontWheel.forceAppPointDistance = 0.001f;
            }

            //////////////////////////////////// acceleration & brake /////////////////////////////////////////////////////////////
            //////////////////////////////////// ACCELERATE /////////////////////////////////////////////////////////////
            if (!crashed && outsideControls.Vertical > 0 && !isReverseOn)
            {//case with acceleration from 0.0f to 0.9f throttle
                coll_frontWheel.brakeTorque = 0;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".
                coll_rearWheel.motorTorque = LegsPower * outsideControls.Vertical;

                // debug - rear wheel is green when accelerate
                meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

                // when normal accelerating CoM z is averaged
                var tmp_cs5 = CoM.localPosition;
                tmp_cs5.z = 0.0f + tmpMassShift;
                tmp_cs5.y = normalCoM;
                CoM.localPosition = tmp_cs5;
                GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
            }
            //case for reverse
            if (!crashed && outsideControls.Vertical > 0 && isReverseOn)
            {
                coll_rearWheel.motorTorque = LegsPower * -outsideControls.Vertical / 2 + (bikeSpeed * 50);//need to make reverse really slow

                // debug - rear wheel is green when accelerate
                meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

                // when normal accelerating CoM z is averaged
                var tmp_cs6 = CoM.localPosition;
                tmp_cs6.z = 0.0f + tmpMassShift;
                tmp_cs6.y = normalCoM;
                CoM.localPosition = tmp_cs6;
                GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
            }

            //////////////////////////////////// ACCELERATE 'full throttle - manual' ///////////////////////////////////////////////////////
            if (!crashed && outsideControls.Vertical > 0.9f && !isReverseOn)// acceleration >0.9f throttle for wheelie	
            {
                coll_frontWheel.brakeTorque = 0;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".
                coll_rearWheel.motorTorque = LegsPower * 1.2f;//1.2f mean it's full throttle
                meshRearWheel.GetComponent<Renderer>().material.color = Color.green;
                GetComponent<Rigidbody>().angularDrag = 20;//for wheelie stability

                CoM.localPosition = new Vector3(CoM.localPosition.z, CoM.localPosition.y, -(1.38f - baseDistance / 1.4f) + tmpMassShift);
                //still working on best wheelie code

                float stoppieEmpower = (bikeSpeed / 3) / 100;
                // need to supress wheelie when leaning because it's always fall and it't not fun at all
                float angleLeanCompensate = 0.0f;
                if (this.transform.localEulerAngles.z < 70)
                {
                    angleLeanCompensate = this.transform.localEulerAngles.z / 30;
                    if (angleLeanCompensate > 0.5f)
                    {
                        angleLeanCompensate = 0.5f;
                    }
                }
                if (this.transform.localEulerAngles.z > 290)
                {
                    angleLeanCompensate = (360 - this.transform.localEulerAngles.z) / 30;
                    if (angleLeanCompensate > 0.5f)
                    {
                        angleLeanCompensate = 0.5f;
                    }
                }

                if (stoppieEmpower + angleLeanCompensate > 0.5f)
                {
                    stoppieEmpower = 0.5f;
                }

                CoM.localPosition = new Vector3(CoM.localPosition.x, -(0.995f - baseDistance / 2.8f) - stoppieEmpower, CoM.localPosition.z);

            
                GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);

                //this is attenuation for rear suspension targetPosition
                //I've made it to prevent very strange launch to sky when wheelie in new Phys3

                var tmpSpsSprg01 = coll_rearWheel.suspensionSpring;//dumper for wheelie jumps
                tmpSpsSprg01.spring = 200000;
                coll_rearWheel.suspensionSpring = tmpSpsSprg01;

            }
            else RearSuspensionRestoration();


            //////////////////////////////////// BRAKING /////////////////////////////////////////////////////////////
            //////////////////////////////////// front brake /////////////////////////////////////////////////////////
            int springWeakness = 0;
            if (!crashed && outsideControls.Vertical < 0 && !isFrontWheelInAir)
            {

                coll_frontWheel.brakeTorque = frontBrakePower * -outsideControls.Vertical;
                coll_rearWheel.motorTorque = 0; // you can't do accelerate and braking same time.

                //more user firendly gomeotric progession braking. But less stoppie and fun :( Boring...

                if (bikeSpeed > 1)
                {// no CoM pull up when speed is zero

                    //when rear brake is used it helps a little to prevent stoppie. Because in real life bike "stretch" a little when you using rear brake just moment before front.
                    float rearBrakeAddon = 0.0f;

                    var tmp_cs11 = CoM.localPosition;
                    tmp_cs11.y += (frontBrakePower / 200000) + tmpMassShift / 50f - rearBrakeAddon;
                    tmp_cs11.z += 0.0025f;
                    CoM.localPosition = tmp_cs11;

                }
                else if (bikeSpeed <= 1 && !crashed && this.transform.localEulerAngles.z < 45 || bikeSpeed <= 1 && !crashed && this.transform.localEulerAngles.z > 315)
                {
                    if (this.transform.localEulerAngles.x < 5 || this.transform.localEulerAngles.x > 355)
                    {
                        var tmp_cs12 = CoM.localPosition;
                        tmp_cs12.y = normalCoM;
                        CoM.localPosition = tmp_cs12;
                    }
                }

                if (CoM.localPosition.y >= -0.2f)
                {
                    var tmp_cs13 = CoM.localPosition;
                    tmp_cs13.y = -0.2f;
                    CoM.localPosition = tmp_cs13;
                }

                if (CoM.localPosition.z >= 0.2f + (GetComponent<Rigidbody>().mass / 500))
                {
                    CoM.localPosition = new Vector3(CoM.localPosition.x, 0.2f + (GetComponent<Rigidbody>().mass / 500), CoM.localPosition.z);
                }

                //////////// 
                //this is attenuation for front suspension when forge spring is compressed
                //I've made it to prevent very strange launch to sky when wheelie in new Phys3
                //problem is launch bike to sky when spring must expand from compressed state. In real life front forge can't create such force.
                float maxFrontSuspConstrain;//temporary variable to make constrain for attenuation ususpension(need to make it always ~15% of initial force) 
                maxFrontSuspConstrain = CoM.localPosition.z;
                if (maxFrontSuspConstrain >= 0.5f) maxFrontSuspConstrain = 0.5f;
                springWeakness = (int)(normalFrontSuspSpring - (normalFrontSuspSpring * 1.5f) * maxFrontSuspConstrain);


            
                GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
                // debug - wheel is red when braking
                meshFrontWheel.GetComponent<Renderer>().material.color = Color.red;

                //we need to mark suspension as very compressed to make it weaker
                forgeBlocked = true;
            }
            else FrontSuspensionRestoration(springWeakness);//here is function for weak front spring and return it's force slowly


            //////////////////////////////////// rear brake /////////////////////////////////////////////////////////
            // rear brake - it's all about lose side stiffness more and more till rear brake is pressed
            


                coll_rearWheel.brakeTorque = 0;

                stiffPowerGain = stiffPowerGain -= 0.05f;
                if (stiffPowerGain < 0)
                {
                    stiffPowerGain = 0;
                }
                var tmp_cs17 = coll_rearWheel.sidewaysFriction;
                tmp_cs17.stiffness = 1.0f - stiffPowerGain;
                coll_rearWheel.sidewaysFriction = tmp_cs17;// side stiffness is back to 2
                var tmp_cs18 = coll_frontWheel.sidewaysFriction;
                tmp_cs18.stiffness = 1.0f - stiffPowerGain;
                coll_frontWheel.sidewaysFriction = tmp_cs18;// side stiffness is back to 1



            //////////////////////////////////// turnning /////////////////////////////////////////////////////////////			
            // there is MOST trick in the code
            // the Unity physics isn't like real life. Wheel collider isn't round as real bike tyre.
            // so, face it - you can't reach accurate and physics correct countersteering effect on wheelCollider
            // For that and many other reasons we restrict front wheel turn angle when when speed is growing
            //(honestly, there was a time when MotoGP bikes has restricted wheel bar rotation angle by 1.5f degree ! as we got here :)			
            tempMaxWheelAngle = wheelbarRestrictCurve.Evaluate(bikeSpeed);//associate speed with curve which you've tuned in Editor

            if (!crashed && outsideControls.Horizontal != 0)
            {

                // while speed is high, wheelbar is restricted 
                coll_frontWheel.steerAngle = tempMaxWheelAngle * outsideControls.Horizontal;
                steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler(0, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);
            }
            else coll_frontWheel.steerAngle = 0;
            

            //auto back CoM when any key not pressed
            if (!crashed && outsideControls.Vertical == 0  && !linkToStunt.stuntIsOn || (outsideControls.Vertical < 0 && isFrontWheelInAir))
            {
                var tmp_cs23 = CoM.localPosition;
                tmp_cs23.y = normalCoM;
                tmp_cs23.z = 0.0f + tmpMassShift;
                CoM.localPosition = tmp_cs23;
                coll_frontWheel.motorTorque = 0;
                coll_frontWheel.brakeTorque = 0;
                coll_rearWheel.motorTorque = 0;
                coll_rearWheel.brakeTorque = 0;
                _rigidbody.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
            }

            /////////////////////////////////////////////////////// RESTART KEY ///////////////////////////////////////////////////////////
            // Restart key - recreate bike few meters above current place
            if (outsideControls.restartBike)
            {

                crashed = false;
                transform.position += new Vector3(0, 0.1f, 0);
                transform.rotation = Quaternion.Euler(0.0f, transform.localEulerAngles.y, 0.0f);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                var tmp_cs26 = CoM.localPosition;
                tmp_cs26.x = 0.0f;
                tmp_cs26.y = normalCoM;
                tmp_cs26.z = 0.0f;
                CoM.localPosition = tmp_cs26;
                //for fix bug when front wheel IN ground after restart(sorry, I really don't understand why it happens);
                coll_frontWheel.motorTorque = 0;
                coll_frontWheel.brakeTorque = 0;
                coll_rearWheel.motorTorque = 0;
                coll_rearWheel.brakeTorque = 0;
                _rigidbody.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
            }

            CrashHappened();
        }

        private void CrashHappened()
        {
            if ((this.transform.localEulerAngles.z >= crashAngle01 && this.transform.localEulerAngles.z <= crashAngle02) && !linkToStunt.stuntIsOn || (this.transform.localEulerAngles.x >= crashAngle03 && this.transform.localEulerAngles.x <= crashAngle04 && !linkToStunt.stuntIsOn))
            {
                _rigidbody.drag = 0.1f; // when 250 bike can easy beat 200km/h // ~55 m/s
                _rigidbody.angularDrag = 0.01f;
                crashed = true;
                var tmp_cs27 = CoM.localPosition;
                tmp_cs27.x = 0.0f;
                tmp_cs27.y = CoMWhenCrahsed;//move CoM a little bit up for funny bike rotations when fall
                tmp_cs27.z = 0.0f;
                CoM.localPosition = tmp_cs27;
                _rigidbody.centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
            }

            if (crashed) coll_rearWheel.motorTorque = 0;//to prevent some bug when bike crashed but still accelerating
        }

        private void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }

            Transform visualWheel = collider.transform.GetChild(0);
            wheelCCenter = collider.transform.TransformPoint(collider.center);


            //dpn't need movement of rear suspension because MTB have no rear suspension
            if (!rearPend)
            {//case where MTB have no rear suspension
                if (collider.gameObject.name != "coll_rear_wheel")
                {
                    if (Physics.Raycast(wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius))
                    {
                        visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
                        if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;
                    }
                    else
                    {
                        visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                        if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
                    }
                }
            }
            else
            {//case where bicycle has sull suspension
                if (Physics.Raycast(wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius))
                {
                    visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
                    if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;

                }
                else
                {
                    visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                    if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
                }

            }

            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            collider.GetWorldPose(out position, out rotation);

            visualWheel.localEulerAngles = new Vector3(visualWheel.localEulerAngles.x, collider.steerAngle - visualWheel.localEulerAngles.z, visualWheel.localEulerAngles.z);
            visualWheel.Rotate(collider.rpm / 60 * 360 * Time.deltaTime, 0.0f, 0.0f);

        }
        private void RearSuspensionRestoration()
        {
            var tmpRearSusp = coll_rearWheel.suspensionSpring;
            tmpRearSusp.spring = normalRearSuspSpring;
            coll_rearWheel.suspensionSpring = tmpRearSusp;   
        }
        private void FrontSuspensionRestoration(int sprWeakness)
        {
            if (forgeBlocked)
            {//supress front spring power to avoid too much force back
                var tmpFrntSusp = coll_frontWheel.suspensionSpring;
                tmpFrntSusp.spring = sprWeakness;
                coll_frontWheel.suspensionSpring = tmpFrntSusp;
                forgeBlocked = false;
            }
            if (coll_frontWheel.suspensionSpring.spring < normalFrontSuspSpring)
            {//slowly returning force to front spring
                var tmpFrntSusp2 = coll_frontWheel.suspensionSpring;
                tmpFrntSusp2.spring += 500.0f;
                coll_frontWheel.suspensionSpring = tmpFrntSusp2;
            }
        }
    }
}