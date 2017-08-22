using UnityEngine;
using System.Collections;



// 1. You can determine in which order your objects are drawn using the Queue tag. 
//    A Shader decides which render queue its objects belong to, this way any Transparent shaders make sure 
//    they are drawn after all opaque objects and so on.
//	
// 2. There are four pre-defined render queues, but there can be more queues in between the predefined ones. 
//    The predefined queues are:
//    a. Background - 
//       This render queue is rendered before any others. It is used for skyboxes and the like.
//    b. Geometry (default) - 
//       This is used for most objects. Opaque geometry uses this queue.
//    c. AlphaTest - 
//       Alpha tested geometry uses this queue. It’s a separate queue from Geometry one since it’s more 
//       efficient to render alpha-tested objects after all solid ones are drawn.
//    d. Transparent - 
//       This render queue is rendered after Geometry and AlphaTest, in back-to-front order. Anything 
//       alpha-blended (i.e. shaders that don’t write to depth buffer) should go here (glass, particle effects).
//    e. Overlay - 
//       This render queue is meant for overlay effects. Anything rendered last should go here (e.g. lens flares).
//       Geometry render queue optimizes the drawing order of the objects for best performance (sort objects by material). 
//    All other render queues sort objects by distance, starting rendering from the furthest ones and ending with 
//    the closest ones.
//
// 3. For special uses in-between queues can be used. Internally each queue is represented by integer index; 
//    Background is 1000, Geometry is 2000, AlphaTest is 2450, Transparent is 3000 and Overlay is 4000. If a 
//    shader uses a queue like this: Tags { "Queue" = "Geometry+1" }
//    This will make the object be rendered after all opaque objects, but before transparent objects, as render 
//    queue index will be 2001 (geometry plus one). This is useful in situations where you want some objects be 
//    always drawn between other sets of objects. For example, in most cases transparent water should be drawn 
//    after opaque objects but before transparent objects.
// 
// 4. By default, materials use render queue of the shader it uses. You can override the render queue used using 
//    "renderer.sharedMaterial". Note that once render queue is set on the materila, it stays at that value, even 
//    if shader is later changed to be different. 
[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour 
{
	// render queue number should be positive to work properly
    public int renderQueue = 3000;

    public Renderer m_renderer;

	// Called for initialization
    private void Start()
    {
        /*
            // Once we set the RenderQueue, the Material asset stores the value in the asset file. 
            // So to improve the performance, remove this component after using it in the Edit mode.
            if (true == Application.isPlaying)
            {
                Debug.LogError ("Remove me before getting into the Play mode!");
                Destroy (this);
            }
            else
            {
                Update ();
            }
        */
        m_renderer = GetComponent<Renderer>();

        Update ();
	}



	// Update only once, then disable this component to save performance
	private void Update ()
	{
		if (null != m_renderer && null != m_renderer.sharedMaterial)
		{
            m_renderer.sharedMaterial.renderQueue = renderQueue; 
		} 
		//this.enabled = false; 
	}
}
