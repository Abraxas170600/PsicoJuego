using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DOTweenAnimation))]
public class DTAnimationEditorView : PropertyDrawer
{
    SerializedProperty animation;

    SerializedProperty appendTween, timeToInsert;


    SerializedProperty tfEndValue, vEndValue, fEndValue, useLocalCoords, duration;


    SerializedProperty randomness, elasticity, vibrato;
    SerializedProperty fadeOut, mode;


    SerializedProperty jumpPower, numJumps;


    SerializedProperty componentType, transitionType, deactiveWithFadeOut;


    private void FindProperties(SerializedProperty property)
    {
        animation = property.FindPropertyRelative("animation");

        appendTween = property.FindPropertyRelative("appendTween");
        timeToInsert = property.FindPropertyRelative("timeToInsert");

        tfEndValue = property.FindPropertyRelative("tfEndValue");
        vEndValue = property.FindPropertyRelative("vEndValue");
        fEndValue = property.FindPropertyRelative("fEndValue");
        useLocalCoords = property.FindPropertyRelative("useLocalCoords");
        duration = property.FindPropertyRelative("duration");


        randomness = property.FindPropertyRelative("randomness");
        elasticity = property.FindPropertyRelative("elasticity");
        vibrato = property.FindPropertyRelative("vibrato");
        fadeOut = property.FindPropertyRelative("fadeOut");
        mode = property.FindPropertyRelative("mode");


        jumpPower = property.FindPropertyRelative("jumpPower");
        numJumps = property.FindPropertyRelative("numJumps");


        componentType = property.FindPropertyRelative("componentType");
        transitionType = property.FindPropertyRelative("transitionType");
        deactiveWithFadeOut = property.FindPropertyRelative("deactiveWithFadeOut");
    }

    const float normalHeight = 30;
    float totalHeight;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FindProperties(property);


        EditorGUI.BeginProperty(position, label, property);

        totalHeight = position.y;
        Rect iPosition = new Rect(position.x, position.y, position.width, 0);

        EditorGUI.PropertyField(new Rect(position.x, totalHeight, position.width, normalHeight),
            animation
            );

        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
           appendTween
           );

        if (!appendTween.boolValue)
            EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
               timeToInsert
               );

        AnimationType type = (AnimationType)animation.enumValueIndex;

        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
            duration
            );


        switch (type)
        {
            case AnimationType.MoveNormal:
            case AnimationType.MoveShake:
            case AnimationType.MovePunch:
            case AnimationType.RotateNormal:
            case AnimationType.RotateShake:
            case AnimationType.RotatePunch:
            case AnimationType.ScaleNormal:
            case AnimationType.ScaleShake:
            case AnimationType.ScalePunch:

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    tfEndValue
                );
                if (tfEndValue.objectReferenceValue == null)
                {
                    EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                        vEndValue
                    );
                }

                switch (type)
                {
                    case AnimationType.MoveNormal:
                    case AnimationType.RotateNormal:
                    case AnimationType.ScaleNormal:
                        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                            useLocalCoords
                        );
                        break;
                    case AnimationType.MoveShake:
                    case AnimationType.RotateShake:
                    case AnimationType.ScaleShake:
                        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                            randomness
                        );
                        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                            fadeOut
                        );
                        break;
                    case AnimationType.MovePunch:
                    case AnimationType.RotatePunch:
                    case AnimationType.ScalePunch:
                        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                            elasticity
                        );

                        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                            vibrato
                        );
                        break;
                }


                switch (type)
                {
                    case AnimationType.RotateNormal:
                    case AnimationType.RotateShake:
                    case AnimationType.RotatePunch:
                        EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                            mode
                        );
                        break;
                }




                break;
            case AnimationType.Jump:

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                   tfEndValue
               );
                if (tfEndValue.objectReferenceValue == null)
                {
                    EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                        vEndValue
                    );
                }
                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    useLocalCoords
                );

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    jumpPower
                );

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    numJumps
                );

                break;
            case AnimationType.Fade:

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    fEndValue
                );

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    componentType
                );

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    transitionType
                );

                EditorGUI.PropertyField(new Rect(position.x, totalHeight += normalHeight, position.width, normalHeight),
                    deactiveWithFadeOut
                );

                break;
            default:
                break;
        }



        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        totalHeight = 0;

        totalHeight += normalHeight * 4;

        animation = property.FindPropertyRelative("animation");

        AnimationType index = (AnimationType)animation.enumValueIndex;


        switch (index)
        {
            case AnimationType.MoveNormal:
            case AnimationType.MoveShake:
            case AnimationType.MovePunch:
            case AnimationType.RotateNormal:
            case AnimationType.RotateShake:
            case AnimationType.RotatePunch:
            case AnimationType.ScaleNormal:
            case AnimationType.ScaleShake:
            case AnimationType.ScalePunch:

                totalHeight += normalHeight * 2;

                switch (index)
                {
                    case AnimationType.MoveNormal:
                    case AnimationType.RotateNormal:
                    case AnimationType.ScaleNormal:
                        totalHeight += normalHeight;
                        break;
                    case AnimationType.MoveShake:
                    case AnimationType.RotateShake:
                    case AnimationType.ScaleShake:
                        totalHeight += normalHeight * 2;
                        break;
                    case AnimationType.MovePunch:
                    case AnimationType.RotatePunch:
                    case AnimationType.ScalePunch:
                        totalHeight += normalHeight * 2;
                        break;
                    default:
                        break;
                }


                switch (index)
                {
                    case AnimationType.RotateNormal:
                    case AnimationType.RotateShake:
                    case AnimationType.RotatePunch:
                        totalHeight += normalHeight;
                        break;
                    default:
                        break;
                }




                break;
            case AnimationType.Jump:

                totalHeight += normalHeight * 5;

                break;
            case AnimationType.Fade:

                totalHeight += normalHeight * 4;

                break;
            default:
                break;
        }

        return totalHeight;
    }
}
