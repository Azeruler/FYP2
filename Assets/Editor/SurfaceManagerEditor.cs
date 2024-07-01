//using unityengine;
//using unityeditor;

//[customeditor(typeof(gameobject))]
//public class surfacemanageradder : editor
//{
//    public override void oninspectorgui()
//    {
//        base.oninspectorgui();  // draws the default inspector

//        // add a button in the gameobject inspector
//        if (guilayout.button("add surface manager to all rigidbodies"))
//        {
//            // this code runs when the button is clicked
//            gameobject targetgameobject = (gameobject)target;
//            addsurfacemanagers(targetgameobject);
//        }
//    }

//    private void addsurfacemanagers(gameobject targetgameobject)
//    {
//        // find all rigidbody components in the gameobject and its children
//        rigidbody[] rigidbodies = targetgameobject.getcomponentsinchildren<rigidbody>();
//        foreach (rigidbody rb in rigidbodies)
//        {
//            // add surfacemanager component if it doesn't already exist
//            surfacemanager sm = rb.gameobject.getcomponent<surfacemanager>();
//            if (sm == null)
//            {
//                rb.gameobject.addcomponent<surfacemanager>();
//                debug.log("added surfacemanager to " + rb.gameobject.name);
//            }
//            else
//            {
//                debug.log("surfacemanager already exists on " + rb.gameobject.name);
//            }
//        }
//    }
//}
