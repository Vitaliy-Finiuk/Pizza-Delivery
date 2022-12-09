using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Spline;
using CodeBase.Utility;
using Dreamteck.Splines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.LevelMechanics
{
    public class LevelCreator : MonoBehaviour
    {
        //===================================================================================

        #region FIELDS

        [Header("PLATFORM SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform platformSplineGeneratorPrefab;
        [SerializeField] private Transform startPlatform;
        [SerializeField] private Transform finishPlatformPrefab;

        [Header("SPLINE SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Vector3 splineScale;
        [SerializeField][Range(1, 100)] private int pointZIncreaseValue;
        public Vector3 SplineScale => splineScale;

        [HorizontalLine(3, EColor.Blue)]

        [SerializeField] private bool enableLevel1Exception;
        [ConditionalHide("enableLevel1Exception", true)]
        [SerializeField] private SplineBehaviour level1Spline;

        [HorizontalLine(1, EColor.Blue)]
        [SerializeField] private bool enableLevel2Exception;
        [ConditionalHide("enableLevel2Exception", true)]
        [SerializeField] private SplineBehaviour level2Spline;

        [HorizontalLine(1, EColor.Blue)]
        [SerializeField] private bool enableLevel3Exception;
        [ConditionalHide("enableLevel3Exception", true)]
        [SerializeField] private SplineBehaviour level3Spline;

        [HorizontalLine(1, EColor.Blue)]
        [SerializeField] private bool enableLevel4Exception;
        [ConditionalHide("enableLevel4Exception", true)]
        [SerializeField] private SplineBehaviour level4Spline;

        [HorizontalLine(3, EColor.Blue)]

        [SerializeField] private SplineBehaviour generalLevelSpline;

        [SerializeField] private bool isSplineGoingForward;
        [ConditionalHide("isSplineGoingForward", true)]
        [SerializeField] private float minXSplinePointPosition;
        [ConditionalHide("isSplineGoingForward", true)]
        [SerializeField] private float maxXSplinePointPosition;
        [ConditionalHide("isSplineGoingForward", true)]
        [SerializeField] private float howManyPointsWillBeStraightAtTheEnd;
        [ConditionalHide("isSplineGoingForward", true)]
        [SerializeField] private float howManyPointsWillBeStraightAtTheBeginning;

        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private List<AnimationCurve> splineCurveList;

        private SplineBehaviour _currentSplineBehaviour;
        private Keyframe[] _splineCurveKeys;

        public SplineComputer currentPlatformSplineComputer { get; private set; }
        private List<double> _currentSplinePointPercentList;
        public List<SplinePoint> _currentSplinePointList;
        private List<ChunkPoint> _chunkPointList;
        private int _splinePointCount;
        private List<SplinePoint> _pointsForUseList;
        private List<ChunkType> _currentChunkTypeList;


        [Header("COLLECTIBLE CHUNK SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform[] collectibleChunks;


        [Header("OBSTACLE CHUNK SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform[] obstacleChunks;


        [Header("GATE CHUNK SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform[] gateChunks;


        [Header("STAIR CHUNK SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform[] stairChunks;

        [Header("RANDOM CHUNK SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform[] randomChunks;

        [Header("PLAYER SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform playerPrefab;
        [SerializeField] private Vector2 playerOffsetToPlatform;
        public Transform player { get; private set; }


        [Header("ENVIRONMENT SECTION")]
        [HorizontalLine(3, EColor.Blue)]
        [SerializeField] private Transform[] environmentBuildingPrefabs;

        [SerializeField][Range(0, 1)] private double percentIncreaseRate;

        [SerializeField][Range(0, 50)] private float _maximumXPosition;
        [SerializeField][Range(0, 50)] private float _minimumXPosition;

        [SerializeField][Range(-100, 0)] private float maximumYPosition;
        [SerializeField][Range(-100, 0)] private float minimumYPosition;

        #endregion

        //===================================================================================

        public void GenerateNewLevel()
        {
            SetLists();
            SetSplineVariables();
            CreateSplinePlatform();
            CreatePlayer();
            CreatePlayerIndicator();
            SetChunks();
            CreateChunks();
            CreateFinishPlatform();
            CreateEnvironment();

            LevelManager.Instance.controller.LevelIsCreated();
        }

        //===================================================================================

        private void SetLists()
        {
            _currentChunkTypeList = new List<ChunkType>();
            _currentSplinePointList = new List<SplinePoint>();
            _pointsForUseList = new List<SplinePoint>();
            _currentSplinePointPercentList = new List<double>();
            _chunkPointList = new List<ChunkPoint>();
        }

        //===================================================================================

        private void SetSplineVariables()
        {
            int currentLevel = LevelManager.Instance.data.GetCurrentLevel();

            switch (currentLevel)
            {
                case 1:
                    {
                        if (enableLevel1Exception)
                        {
                            _splinePointCount = level1Spline.chunkType.Length;
                            _currentChunkTypeList.AddRange(level1Spline.chunkType);
                            _currentSplineBehaviour = level1Spline;
                        }
                        else
                        {
                            _splinePointCount = generalLevelSpline.chunkType.Length;
                            _currentChunkTypeList.AddRange(generalLevelSpline.chunkType);
                            _currentSplineBehaviour = generalLevelSpline;
                        }
                        break;
                    }

                case 2:
                    {
                        if (enableLevel2Exception)
                        {
                            _splinePointCount = level2Spline.chunkType.Length;
                            _currentChunkTypeList.AddRange(level2Spline.chunkType);
                            _currentSplineBehaviour = level2Spline;
                        }
                        else
                        {
                            _splinePointCount = generalLevelSpline.chunkType.Length;
                            _currentChunkTypeList.AddRange(generalLevelSpline.chunkType);
                            _currentSplineBehaviour = generalLevelSpline;
                        }
                        break;
                    }

                case 3:
                    {
                        if (enableLevel3Exception)
                        {
                            _splinePointCount = level3Spline.chunkType.Length;
                            _currentChunkTypeList.AddRange(level3Spline.chunkType);
                            _currentSplineBehaviour = level3Spline;
                        }
                        else
                        {
                            _splinePointCount = generalLevelSpline.chunkType.Length;
                            _currentChunkTypeList.AddRange(generalLevelSpline.chunkType);
                            _currentSplineBehaviour = generalLevelSpline;
                        }
                        break;
                    }

                case 4:
                    {
                        if (enableLevel4Exception)
                        {
                            _splinePointCount = level4Spline.chunkType.Length;
                            _currentChunkTypeList.AddRange(level4Spline.chunkType);
                            _currentSplineBehaviour = level4Spline;
                        }
                        else
                        {
                            _splinePointCount = generalLevelSpline.chunkType.Length;
                            _currentChunkTypeList.AddRange(generalLevelSpline.chunkType);
                            _currentSplineBehaviour = generalLevelSpline;
                        }
                        break;
                    }

                default:
                    {
                        _splinePointCount = generalLevelSpline.chunkType.Length;
                        _currentChunkTypeList.AddRange(generalLevelSpline.chunkType);
                        _currentSplineBehaviour = generalLevelSpline;
                        // _currentSplineBehaviour.splineType = SplineType.Straight; //test
                        break;
                    }
            }

#if UNITY_EDITOR
            if (_splinePointCount == 0)
            {
                Debug.LogError("Spline point count is 0. Add new points in the level creator menu for current level.");
            }
#endif
        }

        //===================================================================================

        private void CreateSplinePlatform()
        {
            GeneratePlatformSplineComputer();
            SetPlatformSplinePoints();
            CreateStartPlatform();
        }

        //===================================================================================

        private void SetChunks()
        {
            if (_currentSplineBehaviour.splineType == SplineType.Left ||
                _currentSplineBehaviour.splineType == SplineType.Right)
            {
                _currentChunkTypeList[_currentChunkTypeList.Count / 2 - 1] = ChunkType.TurnPoint;
            }

            for (int chunkIndex = 0; chunkIndex < _splinePointCount; chunkIndex++)
            {
                ChunkPoint newPoint = new ChunkPoint(false, _currentSplinePointPercentList[chunkIndex],
                    _currentChunkTypeList[chunkIndex]);

                _chunkPointList.Add(newPoint);
            }
        }

        //===================================================================================

        private void CreateChunks()
        {
            for (int chunkIndex = 0; chunkIndex < _currentChunkTypeList.Count; chunkIndex++)
            {
                ChunkPoint chunkPoint = _chunkPointList[chunkIndex];
                ChunkType chunkType = chunkPoint.chunkType;
                Transform chunk = null;

                switch (chunkType)
                {
                    case ChunkType.Collectible:
                        chunk = GetRandomCollectibleChunk();
                        break;

                    case ChunkType.Obstacle:
                        chunk = GetRandomObstacleChunk();
                        break;

                    case ChunkType.Stair:
                        chunk = GetRandomStairChunk();
                        break;

                    case ChunkType.Gate:
                        chunk = GetRandomGateChunk();
                        break;
                }

                if (chunk)
                {
                    Transform spawnedChunk = Instantiate(chunk, Vector3.zero, Quaternion.identity,
                        LevelManager.Instance.chunkHolder);

                    SplinePositioner chunkSplinePositioner = spawnedChunk.GetComponent<SplinePositioner>();

                    if (!chunkSplinePositioner)
                    {
                        chunkSplinePositioner = spawnedChunk.gameObject.AddComponent<SplinePositioner>();
                    }

                    chunkSplinePositioner.spline = currentPlatformSplineComputer;
                    chunkSplinePositioner.SetPercent(_chunkPointList[chunkIndex].splinePercent);

                    SetListsForSpawnedChunk(spawnedChunk, chunkPoint);
                }
            }
        }


        //===================================================================================

        private void CreateFinishPlatform()
        {
            if (finishPlatformPrefab)
            {
                Transform finishPlatform = Instantiate(finishPlatformPrefab, Vector3.zero, Quaternion.identity,
                    LevelManager.Instance.platformHolder);

                SplinePositioner finishPlatformSplinePositioner = finishPlatform.GetComponent<SplinePositioner>();

                if (!finishPlatformSplinePositioner)
                {
                    finishPlatformSplinePositioner = finishPlatform.gameObject.AddComponent<SplinePositioner>();
                }

                finishPlatformSplinePositioner.spline = currentPlatformSplineComputer;

                finishPlatform.SetParent(currentPlatformSplineComputer.transform);

                finishPlatformSplinePositioner.motion.rotationOffset = Vector3.zero;

                finishPlatformSplinePositioner.SetPercent(1);
            }
        }

        //===================================================================================

        private void CreateEnvironment()
        {
            GenerateSideBuildings(isRightSide: true);
            GenerateSideBuildings(isRightSide: false);
        }

        //===================================================================================

        private void GenerateSideBuildings(bool isRightSide)
        {
            if (environmentBuildingPrefabs.Length > 0)
            {
                float maximumXPosition = splineScale.x * _maximumXPosition;
                float minimumXPosition = splineScale.x * _minimumXPosition;

                for (double percent = 0; percent < 1; percent += percentIncreaseRate)
                {
                    Transform building =
                        Instantiate(environmentBuildingPrefabs[Random.Range(0, environmentBuildingPrefabs.Length)], LevelManager.Instance.environmentHolder);

                    SplinePositioner splinePositioner = building.GetComponent<SplinePositioner>();

                    if (!splinePositioner)
                    {
                        splinePositioner = building.gameObject.AddComponent<SplinePositioner>();
                    }

                    splinePositioner.spline = currentPlatformSplineComputer;
                    splinePositioner.SetPercent(percent);

                    splinePositioner.motion.offset = isRightSide
                        ? Vector2.right * Random.Range(minimumXPosition, maximumXPosition)
                        : Vector2.left * Random.Range(minimumXPosition, maximumXPosition);

                    splinePositioner.motion.offset += Vector2.up * Random.Range(minimumYPosition, maximumYPosition);
                }
            }
        }

        //===================================================================================

        private void GeneratePlatformSplineComputer()
        {
            if (platformSplineGeneratorPrefab)
            {
                Transform spline = Instantiate(platformSplineGeneratorPrefab, Vector3.zero,
                    Quaternion.identity, LevelManager.Instance.platformHolder);

                if (spline)
                {
                    currentPlatformSplineComputer = spline.GetComponent<SplineComputer>();
                    SplineMesh currentSplineMesh = spline.gameObject.GetComponent<SplineMesh>();

                    if (!currentPlatformSplineComputer)
                    {
                        currentPlatformSplineComputer =
                            currentPlatformSplineComputer.gameObject.AddComponent<SplineComputer>();
                    }

                    if (!currentSplineMesh)
                    {
                        currentSplineMesh = currentPlatformSplineComputer.gameObject.AddComponent<SplineMesh>();
                    }

                    currentSplineMesh.GetChannel(0).GetMesh(0).scale = splineScale;
                }
            }
        }

        //===================================================================================

        private void SetPlatformSplinePoints()
        {
            if (_currentSplineBehaviour != null)
            {
                SplineType splineType = _currentSplineBehaviour.splineType;

            DetermineSplineType:
                switch (splineType)
                {
                    case SplineType.Straight:
                        {
                            _currentSplineBehaviour.splineType = SplineType.Straight;
                            SetSplinePointsStraight();
                            break;
                        }

                    case SplineType.Left:
                        {
                            _currentSplineBehaviour.splineType = SplineType.Left;
                            SetSplinePointsLeft();
                            break;
                        }

                    case SplineType.Right:
                        {
                            _currentSplineBehaviour.splineType = SplineType.Right;
                            SetSplinePointsRight();
                            break;
                        }

                    case SplineType.UpAndDown:
                        {
                            _currentSplineBehaviour.splineType = SplineType.UpAndDown;
                            SetSplinePointsUpAndDown();
                            break;
                        }

                    case SplineType.Random:
                        {
                            Array values = Enum.GetValues(typeof(SplineType));
                            int maxIndex = Enum.GetValues(typeof(SplineType)).Cast<int>().Max();
                            int randomIndex = UnityEngine.Random.Range(0, maxIndex);
                            splineType = (SplineType)values.GetValue(randomIndex);
                            goto DetermineSplineType;
                        }
                }

                // ReversePointOrder(currentPlatformSplineComputer);

                SetSplineLists();
            }
        }

        //===================================================================================

        private void ReversePointOrder(SplineComputer spline)
        {
            SplinePoint[] points = spline.GetPoints();
            for (int i = 0; i < Mathf.FloorToInt(points.Length / 2); i++)
            {
                SplinePoint temp = points[i];
                points[i] = points[(points.Length - 1) - i];
                Vector3 tempTan = points[i].tangent;
                points[i].tangent = points[i].tangent2;
                points[i].tangent2 = tempTan;
                int oppositeIndex = (points.Length - 1) - i;
                points[oppositeIndex] = temp;
                tempTan = points[oppositeIndex].tangent;
                points[oppositeIndex].tangent = points[oppositeIndex].tangent2;
                points[oppositeIndex].tangent2 = tempTan;
            }
            if (points.Length % 2 != 0)
            {
                Vector3 tempTan = points[Mathf.CeilToInt(points.Length / 2)].tangent;
                points[Mathf.CeilToInt(points.Length / 2)].tangent = points[Mathf.CeilToInt(points.Length / 2)].tangent2;
                points[Mathf.CeilToInt(points.Length / 2)].tangent2 = tempTan;
            }
            spline.SetPoints(points);
        }

        //===================================================================================

        private void CreateStartPlatform()
        {
            if (startPlatform)
            {
                Transform platform = Instantiate(startPlatform, currentPlatformSplineComputer.GetPoint(0).position + Vector3.back * 10, Quaternion.identity, LevelManager.Instance.platformHolder);

                platform.localScale = splineScale;
            }
        }

        //===================================================================================

        private void SetSplinePointsStraight()
        {
            bool isLeftAndRightEnabled = _currentSplineBehaviour.isLeftAndRightEnabled;

            for (int splinePoint = 0; splinePoint < _splinePointCount; splinePoint++)
            {
                float splinePointPosXValue = 0;

                if (isSplineGoingForward)
                {
                    if (isLeftAndRightEnabled)
                    {
                        if (splinePoint < howManyPointsWillBeStraightAtTheBeginning)
                        {
                            splinePointPosXValue = 0;
                        }
                        else
                        {
                            splinePointPosXValue = splinePoint >= (_splinePointCount - howManyPointsWillBeStraightAtTheEnd) + 1
                                ? 0
                                : Random.Range(minXSplinePointPosition, maxXSplinePointPosition);
                        }
                    }
                }

                Vector3 splinePointPos = new Vector3(
                    splinePointPosXValue,
                    0,
                    splinePoint * pointZIncreaseValue);

                currentPlatformSplineComputer.SetPoint(splinePoint, new SplinePoint(splinePointPos));
            }
        }

        //===================================================================================

        private void SetSplinePointsLeft()
        {
            int turnPoint = generalLevelSpline.chunkType.Length / 2 - 1;
            generalLevelSpline.chunkType[turnPoint] = ChunkType.TurnPoint;

            Vector3 splinePointOnZPos = Vector3.zero;

            for (int splinePoint = 0; splinePoint < _splinePointCount / 2; splinePoint++)
            {
                splinePointOnZPos = Vector3.forward * (splinePoint * pointZIncreaseValue);
                currentPlatformSplineComputer.SetPoint(splinePoint, new SplinePoint(splinePointOnZPos));
            }

            for (int splinePoint = 0; splinePoint < _splinePointCount / 2; splinePoint++)
            {
                Vector3 splinePointOnXPos = Vector3.left * ((splinePoint + 1) * pointZIncreaseValue);
                Vector3 finalPointPos = new Vector3(splinePointOnXPos.x, 0, splinePointOnZPos.z);
                currentPlatformSplineComputer.SetPoint(splinePoint + _splinePointCount / 2, new SplinePoint(finalPointPos));
            }
        }

        //===================================================================================

        private void SetSplinePointsRight()
        {
            int turnPoint = generalLevelSpline.chunkType.Length / 2 - 1;
            generalLevelSpline.chunkType[turnPoint] = ChunkType.TurnPoint;

            Vector3 splinePointOnZPos = Vector3.zero;

            for (int splinePoint = 0; splinePoint < _splinePointCount / 2; splinePoint++)
            {
                splinePointOnZPos = Vector3.forward * (splinePoint * pointZIncreaseValue);
                currentPlatformSplineComputer.SetPoint(splinePoint, new SplinePoint(splinePointOnZPos));
            }

            for (int splinePoint = 0; splinePoint < _splinePointCount / 2; splinePoint++)
            {
                Vector3 splinePointOnXPos = Vector3.right * ((splinePoint + 1) * pointZIncreaseValue);
                Vector3 finalPointPos = new Vector3(splinePointOnXPos.x, 0, splinePointOnZPos.z);
                currentPlatformSplineComputer.SetPoint(splinePoint + _splinePointCount / 2, new SplinePoint(finalPointPos));
            }
        }

        //===================================================================================

        private void SetSplinePointsUpAndDown()
        {
            AnimationCurve currentSplineCurve = splineCurveList[Random.Range(0, splineCurveList.Count)];
            _splineCurveKeys = currentSplineCurve.keys;
            bool isLeftAndRightEnabled = _currentSplineBehaviour.isLeftAndRightEnabled;

            for (int splinePoint = 0; splinePoint < _splinePointCount; splinePoint++)
            {
                float splinePointPosXValue = 0;

                if (isLeftAndRightEnabled)
                {
                    if (splinePoint < howManyPointsWillBeStraightAtTheBeginning)
                    {
                        splinePointPosXValue = 0;
                    }
                    else
                    {
                        splinePointPosXValue = splinePoint >= _splinePointCount - howManyPointsWillBeStraightAtTheEnd + 1
                            ? 0
                            : Random.Range(minXSplinePointPosition, maxXSplinePointPosition);
                    }
                }

                Vector3 splinePointPos = new Vector3(
                    splinePointPosXValue,
                    _splineCurveKeys[splinePoint].value * 100,
                    splinePoint * pointZIncreaseValue);

                currentPlatformSplineComputer.SetPoint(splinePoint, new SplinePoint(splinePointPos));
            }
        }

        //===================================================================================

        private void SetSplineLists()
        {
            SplinePoint[] allPoints = currentPlatformSplineComputer.GetPoints();
            _currentSplinePointList.AddRange(allPoints);
            _pointsForUseList.AddRange(_currentSplinePointList);

            List<SplinePoint> splinePointList = new List<SplinePoint>();
            splinePointList.AddRange(_currentSplinePointList);

            for (int splinePoint = 0; splinePoint < _splinePointCount; splinePoint++)
            {
                SplinePoint point = splinePointList[0];
                splinePointList.RemoveAt(0);
                int indexOfPoint = 0;

                for (int i = 0; i < allPoints.Length; i++)
                {
                    if (Equals(allPoints[i], point))
                    {
                        indexOfPoint = i;
                        break;
                    }
                }

                double pointPercent = currentPlatformSplineComputer.GetPointPercent(indexOfPoint);
                _currentSplinePointPercentList.Add(pointPercent);
            }
        }

        //===================================================================================

        private void CreatePlayer()
        {
            if (playerPrefab)
            {
                player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity,
                    LevelManager.Instance.playerHolder);

                if (player)
                {
                    SplineFollower playerSplineFollower = player.GetComponent<SplineFollower>();

                    if (!playerSplineFollower)
                    {
                        playerSplineFollower = player.gameObject.AddComponent<SplineFollower>();
                    }

                    playerSplineFollower.spline = currentPlatformSplineComputer;
                    playerSplineFollower.motion.offset = playerOffsetToPlatform;
                }
            }
        }

        //===================================================================================

        private void CreatePlayerIndicator()
        {
            //if (playerIndicatorPrefab)
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        Transform indicator = Instantiate(playerIndicatorPrefab, LevelManager.Instance.playerHolder);

            //        if (indicator)
            //        {
            //            PlayerIndicatorController controller = indicator.GetComponent<PlayerIndicatorController>();

            //            if (!controller)
            //            {
            //                controller = indicator.gameObject.AddComponent<PlayerIndicatorController>();
            //            }

            //            controller.offsetToPlayer = PlayerController.Instance.playerStackController
            //                .stackPointList[i].stackPosition;

            //            SplinePositioner positioner = indicator.gameObject.GetComponent<SplinePositioner>();

            //            if (!positioner)
            //            {
            //                positioner = indicator.gameObject.AddComponent<SplinePositioner>();
            //            }

            //            positioner.spline = currentPlatformSplineComputer;
            //        }
            //    }
            //}
        }

        //===================================================================================
        //===================================================================================
        //===================================================================================

        private Transform GetRandomCollectibleChunk()
        {
            return collectibleChunks.Length > 0
                ? collectibleChunks[Random.Range(0, collectibleChunks.Length)]
                : null;
        }

        //===================================================================================

        private Transform GetRandomObstacleChunk()
        {
            return obstacleChunks.Length > 0
                ? obstacleChunks[Random.Range(0, obstacleChunks.Length)]
                : null;
        }

        //===================================================================================

        private Transform GetRandomGateChunk()
        {
            return gateChunks.Length > 0
                ? gateChunks[Random.Range(0, gateChunks.Length)]
                : null;
        }

        //===================================================================================

        private Transform GetRandomStairChunk()
        {
            return stairChunks.Length > 0
                ? stairChunks[Random.Range(0, stairChunks.Length)]
                : null;
        }

        //===================================================================================

        private void SetListsForSpawnedChunk(Transform spawnedChunk, ChunkPoint chunkPoint)
        {
            //if (spawnedChunk)
            //{
            //    switch (chunkPoint.chunkType)
            //    {
            //        case ChunkType.Collectible:
            //            {
            //                foreach (Transform collectible in spawnedChunk)
            //                {
            //                    allCollectibleList.Add(collectible);
            //                }
            //                break;
            //            }

            //        case ChunkType.Obstacle:
            //            {
            //                foreach (Transform obstacle in spawnedChunk)
            //                {
            //                    allObstacleList.Add(obstacle);
            //                }
            //                break;
            //            }

            //        case ChunkType.BuyGate:
            //            {
            //                foreach (Transform door in spawnedChunk)
            //                {
            //                    _allGateList.Add(door);
            //                }
            //                break;
            //            }

            //        case ChunkType.Stair:
            //            {
            //                foreach (Transform stair in spawnedChunk)
            //                {
            //                    _allStairList.Add(stair);
            //                }
            //                break;
            //            }
            //    }
            //}
        }

        //===================================================================================


    }
}