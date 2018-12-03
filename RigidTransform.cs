
// Rigid Transform implementation using explanation in http://nghiaho.com/?page_id=671
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using Accord;

public class RigidTransform : MonoBehaviour {
    
    // Initial objects   
    public GameObject CubeReg;
    public GameObject SphereReg;
    public GameObject CylinderReg;

    // Transformed objects
    public GameObject CubeTransformed;
    public GameObject SphereTransformed;
    public GameObject CylinderTransformed;
    
    // object used as a 4th point to test Transformation Matrix
    public GameObject TestObject;
    
    // variables used for RigidTransform
    private Matrix3x3 H;
    private Matrix3x3 U;
    private Accord.Math.Vector3 E;
    private Matrix3x3 V;
    private Matrix3x3 R;
    Accord.Math.Vector3 centroidA;
    Accord.Math.Vector3 centroidB;
    Accord.Math.Vector4 x;
    UnityEngine.Matrix4x4 TransformationMatrix;
    private Accord.Math.Vector3 Translation;

    // Saving initial position and rotation to apply transformation in Update()
    private UnityEngine.Vector3 InitPosition;
    private Quaternion InitQT;

    void Start () {
        InitPosition = TestObject.transform.position;
        InitQT = TestObject.transform.rotation;        
    }
   
    // Calculation of covariance matrix H
    private Matrix3x3 CovarianceMatrixStep( Accord.Math.Vector3 difSetA, Accord.Math.Vector3 difSetB )
    {
        Matrix3x3 M;
        M.V00 = difSetA.X * difSetB.X;
        M.V01 = difSetA.X * difSetB.Y;
        M.V02 = difSetA.X * difSetB.Z;

        M.V10 = difSetA.Y * difSetB.X;
        M.V11 = difSetA.Y * difSetB.Y;
        M.V12 = difSetA.Y * difSetB.Z;

        M.V20 = difSetA.Z * difSetB.X;
        M.V21 = difSetA.Z * difSetB.Y;
        M.V22 = difSetA.Z * difSetB.Z;


        return M;
    }

    // Converting Unity.Vector3 to Accord.Vector3
    private Accord.Math.Vector3 UnitytoAccord( UnityEngine.Vector3 pos )
    {
        Accord.Math.Vector3 posTransformed = new Accord.Math.Vector3();
        posTransformed.X = pos.x;
        posTransformed.Y = pos.y;
        posTransformed.Z = pos.z;

        return posTransformed;
    }
    // Converting  Accord.Vector3 to Unity.Vector3 
    private UnityEngine.Vector3 AccordtoUnity(Accord.Math.Vector3 pos)
    {
        UnityEngine.Vector3 posTransformed = new UnityEngine.Vector3();
        posTransformed.x = pos.X;
        posTransformed.y = pos.Y;
        posTransformed.z = pos.Z;

        return posTransformed;
    }
    private Matrix3x3 ZeroMatrix(Matrix3x3 m)
    {
        m.V00 = 0;
        m.V01 = 0;
        m.V02 = 0;
        m.V10 = 0;
        m.V11 = 0;
        m.V12 = 0;
        m.V20 = 0;
        m.V21 = 0;
        m.V22 = 0;

        return m;
    }
    private Matrix3x3 NegativeMatrix(Matrix3x3 m)
    {
        m.V00 *= (-1);
        m.V01 *= (-1);
        m.V02 *= (-1);
        m.V10 *= (-1);
        m.V11 *= (-1);
        m.V12 *= (-1);
        m.V20 *= (-1);
        m.V21 *= (-1);
        m.V22 *= (-1);

        return m;
    }

    // Creating Unity Transformation matrix using 3x3 Rotation matrix and translation vector acquired from RigidTransform
    UnityEngine.Matrix4x4 AccordToUnityMatrix(UnityEngine.Matrix4x4 UnityM,Accord.Math.Matrix3x3 RotationM,Accord.Math.Vector3 Trans)
    {
              
        UnityM.m00 = RotationM.V00;
        UnityM.m10 = RotationM.V10;
        UnityM.m20 = RotationM.V20;
        
        UnityM.m01 = RotationM.V01;
        UnityM.m11 = RotationM.V11;
        UnityM.m21 = RotationM.V21;
        
        UnityM.m02 = RotationM.V02;
        UnityM.m12 = RotationM.V12;
        UnityM.m22 = RotationM.V22;


        UnityM.m03 = Trans.X;
        UnityM.m13 = Trans.Y;
        UnityM.m23 = Trans.Z;

        UnityM.m30 = 0;
        UnityM.m31 = 0;
        UnityM.m32 = 0;
        UnityM.m33 = 1;

        return UnityM;
    }

    // Function to apply transformation to 4th point/object
    void ApplyTransformation()
    {
        //Calculating Centroids from both coordinate system
        centroidA = (UnitytoAccord(CubeReg.transform.position) + UnitytoAccord(SphereReg.transform.position)
                  + UnitytoAccord(CylinderReg.transform.position)) / 3;
        centroidB = (UnitytoAccord(CubeTransformed.transform.position) + UnitytoAccord(SphereTransformed.transform.position)
                  + UnitytoAccord(CylinderTransformed.transform.position)) / 3;
        //Debug.Log("Centroid A is" + centroidA + " Centroid B is" + centroidB);

        // Calculating Covariance Matrix
        H = CovarianceMatrixStep(UnitytoAccord(CubeReg.transform.position) - centroidA, UnitytoAccord(CubeTransformed.transform.position) - centroidB)
            + CovarianceMatrixStep(UnitytoAccord(SphereReg.transform.position) - centroidA, UnitytoAccord(SphereTransformed.transform.position) - centroidB)
            + CovarianceMatrixStep(UnitytoAccord(CylinderReg.transform.position) - centroidA, UnitytoAccord(CylinderTransformed.transform.position) - centroidB);

        H.SVD(out U, out E, out V);
        R = V * U.Transpose();        

        Translation = NegativeMatrix(R) * centroidA + centroidB;
        //Debug.Log("Translation is" + Translation);
        TransformationMatrix = AccordToUnityMatrix(TransformationMatrix, R, Translation);
        //Debug.Log(TransformationMatrix);
        
        // Transformaiton Matrix for Unity
        TransformationMatrix.SetTRS(AccordtoUnity(Translation), Quaternion.LookRotation(TransformationMatrix.GetColumn(1),
             TransformationMatrix.GetColumn(2)), UnityEngine.Vector3.one);
        
        // Applying Translation and rotation to 4th point/object
        TestObject.transform.position = TransformationMatrix.MultiplyPoint(InitPosition);
        TestObject.transform.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2))* InitQT;



    }
    // Update is called once per frame
    void Update () {           
           ApplyTransformation();        
    }
}
