using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BreakoutAcademy : Academy {

    public float xVariation;
    public float zVariation;

    public Text Fail;
    public Text Success;

    public int failCount;
    public int successCount;


    public override void AcademyReset()
    {
        //xVariation = (float)resetParameters["x_variation"];
        //zVariation = (float)resetParameters["z_variation"];
    }

    public override void AcademyStep()
    {
    }

    private void Update()
    {
        Fail.text = "Fails : " + failCount;
        Success.text = "Successes : " + successCount;
    }
}
