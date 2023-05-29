using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Leaf is the actual action that the AI will perform
public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public Leaf()
    {

    }

    public Leaf(string n , Tick pm)
    {
        name = n;
        ProcessMethod = pm;
    }

    public override Status Process()
    {
        if (ProcessMethod != null)
            return ProcessMethod();
        return Status.FAILURE;
    }
}
