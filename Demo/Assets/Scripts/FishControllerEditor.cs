using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FishController))]
public class FishControllerEditor : Editor
{
    SerializedProperty myProperty;
    SerializedProperty bubbles;

    void OnEnable()
    {
        myProperty = serializedObject.FindProperty("childPrefabs");
        bubbles = serializedObject.FindProperty("bubbles");
        //
        if (bubbles.objectReferenceValue == null)
        {
            bubbles.objectReferenceValue = Transform.FindObjectOfType(typeof(Bubbles));
        }

    }

    public override void OnInspectorGUI()
    {
        Color warningColor = new Color32(255, 174, 0, 255);
        Color warningColor2 = Color.yellow;
        Color dColor = new Color32(175, 175, 175, 255);
        Color aColor = Color.white;
        var warningStyle = new GUIStyle(GUI.skin.label);
        warningStyle.normal.textColor = warningColor;
        warningStyle.fontStyle = FontStyle.Bold;
        var warningStyle2 = new GUIStyle(GUI.skin.label);
        warningStyle2.normal.textColor = warningColor2;
        warningStyle2.fontStyle = FontStyle.Bold;
        GUI.color = dColor;
        //
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        if (UnityEditor.EditorApplication.isPlaying)
        {
            GUI.enabled = false;
        }
        SerializedProperty updateDivisor = serializedObject.FindProperty("updateDivisor");
        float cupdateDivisor = updateDivisor.intValue;
        updateDivisor.intValue = (int)EditorGUILayout.Slider("Frame Skipping", cupdateDivisor, 1f, 10);
        GUI.enabled = true;
        if (updateDivisor.intValue > 4)
        {
            EditorGUILayout.LabelField("Will cause choppy movement", warningStyle);
        }
        else if (updateDivisor.intValue > 2)
        {
            EditorGUILayout.LabelField("Can cause choppy movement	", warningStyle2);
        }
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        //
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        serializedObject.Update();
        EditorGUILayout.PropertyField(myProperty, new GUIContent("Fish Prefabs"), true);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("Prefabs must have SchoolChild component", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        //
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Grouping", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Move fish into a parent transform", EditorStyles.miniLabel);
        SerializedProperty groupChildToSchool = serializedObject.FindProperty("groupChildToSchool");
        groupChildToSchool.boolValue = EditorGUILayout.Toggle("Group to School", groupChildToSchool.boolValue);
        if (groupChildToSchool.boolValue)
        {
            GUI.enabled = false;
        }
        //
        SerializedProperty groupChildToNewTransform = serializedObject.FindProperty("groupChildToNewTransform");
        groupChildToNewTransform.boolValue = EditorGUILayout.Toggle("Group to New GameObject", groupChildToNewTransform.boolValue);
        SerializedProperty groupName = serializedObject.FindProperty("groupName");
        groupName.stringValue = EditorGUILayout.TextField("Group Name", groupName.stringValue);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        //
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Bubbles", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(bubbles, new GUIContent("Bubbles Object"), true);
        if (bubbles.objectReferenceValue)
        {
            Bubbles bubble = bubbles.objectReferenceValue as Bubbles;
            bubble.emitEverySecond = EditorGUILayout.FloatField("Emit Every Second", bubble.emitEverySecond);
            bubble.speedEmitMultplier = EditorGUILayout.FloatField("Fish Speed Emit Multiplier", bubble.speedEmitMultplier);
            bubble.minBubbles = EditorGUILayout.IntField("Minimum Bubbles Emitted", bubble.minBubbles);
            bubble.maxBubbles = EditorGUILayout.IntField("Maximum Bubbles Emitted", bubble.maxBubbles);
            if (GUI.changed)
            {
                EditorUtility.SetDirty(bubble);
            }
        }
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        //
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Area Size", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Size of area the school roams within", EditorStyles.miniLabel);
        SerializedProperty positionSphere = serializedObject.FindProperty("positionSphere");
        positionSphere.floatValue = EditorGUILayout.FloatField("Roaming Area Width", positionSphere.floatValue);
        SerializedProperty positionSphereDepth = serializedObject.FindProperty("positionSphereDepth");
        positionSphereDepth.floatValue = EditorGUILayout.FloatField("Roaming Area Depth", positionSphereDepth.floatValue);
        SerializedProperty positionSphereHeight = serializedObject.FindProperty("positionSphereHeight");
        positionSphereHeight.floatValue = EditorGUILayout.FloatField("Roaming Area Height", positionSphereHeight.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        //
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Size of the school", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Size of area the Fish swim towards", EditorStyles.miniLabel);
        SerializedProperty childAmount = serializedObject.FindProperty("childAmount");
        childAmount.intValue = (int)EditorGUILayout.Slider("Fish Amount", childAmount.intValue, 1, 500);
        SerializedProperty spawnSphere = serializedObject.FindProperty("spawnSphere");
        spawnSphere.floatValue = EditorGUILayout.FloatField("School Width", spawnSphere.floatValue);
        SerializedProperty spawnSphereDepth = serializedObject.FindProperty("spawnSphereDepth");
        spawnSphereDepth.floatValue = EditorGUILayout.FloatField("School Depth", spawnSphereDepth.floatValue);
        SerializedProperty spawnSphereHeight = serializedObject.FindProperty("spawnSphereHeight");
        spawnSphereHeight.floatValue = EditorGUILayout.FloatField("School Height", spawnSphereHeight.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Speed and Movement ", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Change Fish speed, rotation and movement behaviors", EditorStyles.miniLabel);
        SerializedProperty childSpeedMultipler = serializedObject.FindProperty("childSpeedMultipler");
        childSpeedMultipler.floatValue = EditorGUILayout.FloatField("Random Speed Multiplier", childSpeedMultipler.floatValue);
        SerializedProperty SpeedCurveMultiplier = serializedObject.FindProperty("SpeedCurveMultiplier");
        SpeedCurveMultiplier.animationCurveValue = EditorGUILayout.CurveField("Speed Curve Multiplier", SpeedCurveMultiplier.animationCurveValue);
        if (childSpeedMultipler.floatValue < 0.01f) childSpeedMultipler.floatValue = 0.01f;
        SerializedProperty minSpeed = serializedObject.FindProperty("minSpeed");
        minSpeed.floatValue = EditorGUILayout.FloatField("Min Speed", minSpeed.floatValue);
        SerializedProperty maxSpeed = serializedObject.FindProperty("maxSpeed");
        maxSpeed.floatValue = EditorGUILayout.FloatField("Max Speed", maxSpeed.floatValue);
        SerializedProperty acceleration = serializedObject.FindProperty("acceleration");
        acceleration.floatValue = EditorGUILayout.Slider("Fish Acceleration", acceleration.floatValue, 0.001f, 0.07f);
        SerializedProperty brake = serializedObject.FindProperty("brake");
        brake.floatValue = EditorGUILayout.Slider("Fish Brake Power", brake.floatValue, 0.001f, 0.025f);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Rotation Damping", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Bigger number damping will make Fish turn faster", EditorStyles.miniLabel);
        SerializedProperty minDamping = serializedObject.FindProperty("minDamping");
        minDamping.floatValue = EditorGUILayout.FloatField("Min Damping Turns", minDamping.floatValue);
        SerializedProperty maxDamping = serializedObject.FindProperty("maxDamping");
        maxDamping.floatValue = EditorGUILayout.FloatField("Max Damping Turns", maxDamping.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Randomize Fish Size ", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Change scale of Fish when they are added to the stage", EditorStyles.miniLabel);
        SerializedProperty minScale = serializedObject.FindProperty("minScale");
        minScale.floatValue = EditorGUILayout.FloatField("Min Scale", minScale.floatValue);
        SerializedProperty maxScale = serializedObject.FindProperty("maxScale");
        maxScale.floatValue = EditorGUILayout.FloatField("Max Scale", maxScale.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Fish Random Animation Speeds", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Animation speeds are also increased by movement speed", EditorStyles.miniLabel);
        SerializedProperty minAnimationSpeed = serializedObject.FindProperty("minAnimationSpeed");
        minAnimationSpeed.floatValue = EditorGUILayout.FloatField("Min Animation Speed", minAnimationSpeed.floatValue);
        SerializedProperty maxAnimationSpeed = serializedObject.FindProperty("maxAnimationSpeed");
        maxAnimationSpeed.floatValue = EditorGUILayout.FloatField("Max Animation Speed", maxAnimationSpeed.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Fish Waypoint Distance", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Waypoints inside small sphere", EditorStyles.miniLabel);
        SerializedProperty waypointDistance = serializedObject.FindProperty("waypointDistance");
        waypointDistance.floatValue = EditorGUILayout.FloatField("Distance To Waypoint", waypointDistance.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Fish Triggers School Waypoint", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Fish waypoint triggers a new School waypoint", EditorStyles.miniLabel);
        SerializedProperty childTriggerPos = serializedObject.FindProperty("childTriggerPos");
        childTriggerPos.boolValue = EditorGUILayout.Toggle("Fish Trigger Waypoint", childTriggerPos.boolValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Automaticly New Waypoint", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Automaticly trigger new school waypoint", EditorStyles.miniLabel);
        SerializedProperty autoRandomPosition = serializedObject.FindProperty("autoRandomPosition");
        autoRandomPosition.boolValue = EditorGUILayout.Toggle("Auto School Waypoint", autoRandomPosition.boolValue);
        if (autoRandomPosition.boolValue)
        {
            SerializedProperty randomPositionTimerMin = serializedObject.FindProperty("randomPositionTimerMin");
            randomPositionTimerMin.floatValue = EditorGUILayout.FloatField("Min Delay", randomPositionTimerMin.floatValue);
            SerializedProperty randomPositionTimerMax = serializedObject.FindProperty("randomPositionTimerMax");
            randomPositionTimerMax.floatValue = EditorGUILayout.FloatField("Max Delay", randomPositionTimerMax.floatValue);
            if (randomPositionTimerMin.floatValue < 1)
            {
                randomPositionTimerMin.floatValue = 1;
            }
            if (randomPositionTimerMax.floatValue < 1)
            {
                randomPositionTimerMax.floatValue = 1;
            }
        }
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Fish Force School Waypoint", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Force all Fish to change waypoints when school changes waypoint", EditorStyles.miniLabel);
        SerializedProperty forceChildWaypoints = serializedObject.FindProperty("forceChildWaypoints");
        forceChildWaypoints.boolValue = EditorGUILayout.Toggle("Force Fish Waypoints", forceChildWaypoints.boolValue);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Force New Waypoint Delay", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("How many seconds until the Fish in school will change waypoint", EditorStyles.miniLabel);
        SerializedProperty forcedRandomDelay = serializedObject.FindProperty("forcedRandomDelay");
        forcedRandomDelay.floatValue = EditorGUILayout.FloatField("Waypoint Delay", forcedRandomDelay.floatValue);
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        EditorGUILayout.LabelField("Obstacle Avoidance", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Steer and push away from obstacles (uses more CPU)", EditorStyles.miniLabel);
        SerializedProperty avoidance = serializedObject.FindProperty("avoidance");
        avoidance.boolValue = EditorGUILayout.Toggle("Avoidance (enable/disable)", avoidance.boolValue);
        if (avoidance.boolValue)
        {
            SerializedProperty avoidAngle = serializedObject.FindProperty("avoidance");
            avoidAngle.floatValue = EditorGUILayout.Slider("Avoid Angle", avoidAngle.floatValue, 0.05f, 0.95f);
            SerializedProperty avoidDistance = serializedObject.FindProperty("avoidDistance");
            avoidDistance.floatValue = EditorGUILayout.FloatField("Avoid Distance", avoidDistance.floatValue);
            if (avoidDistance.floatValue <= 0.1) avoidDistance.floatValue = 0.1f;
            SerializedProperty avoidSpeed = serializedObject.FindProperty("avoidSpeed");
            avoidSpeed.floatValue = EditorGUILayout.FloatField("Avoid Speed", avoidSpeed.floatValue);
            SerializedProperty stopDistance = serializedObject.FindProperty("stopDistance");
            stopDistance.floatValue = EditorGUILayout.FloatField("Stop Distance", stopDistance.floatValue);
            SerializedProperty stopSpeedMultiplier = serializedObject.FindProperty("stopSpeedMultiplier");
            stopSpeedMultiplier.floatValue = EditorGUILayout.FloatField("Stop Speed Multiplier", stopSpeedMultiplier.floatValue);
            if (stopDistance.floatValue <= 0.1f) stopDistance.floatValue = 0.1f;
        }
        EditorGUILayout.EndVertical();
        //
        GUI.color = dColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.color = Color.white;
        SerializedProperty push = serializedObject.FindProperty("push");
        push.boolValue = EditorGUILayout.Toggle("Push (enable/disable)", push.boolValue);
        if (push.boolValue)
        {
            SerializedProperty pushDistance = serializedObject.FindProperty("pushDistance");
            pushDistance.floatValue = EditorGUILayout.FloatField("Push Distance", pushDistance.floatValue);
            if (pushDistance.floatValue <= 0.1f) pushDistance.floatValue = 0.1f;
            SerializedProperty pushForce = serializedObject.FindProperty("pushForce");
            pushForce.floatValue = EditorGUILayout.FloatField("Push Force", pushForce.floatValue);
            if (pushForce.floatValue <= 0.01f) pushForce.floatValue = 0.01f;
        }
        EditorGUILayout.EndVertical();
        //
        if (GUI.changed) EditorUtility.SetDirty(target);

        serializedObject.ApplyModifiedProperties();
    }
}
