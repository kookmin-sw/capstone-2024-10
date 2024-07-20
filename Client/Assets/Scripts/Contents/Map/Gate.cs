using Fusion;

public class Gate : NetworkBehaviour
{
    public void Open()
    {
        GetComponent<NetworkMecanimAnimator>().Animator.SetBool("IsOpen", true);
    }

    public void Close()
    {
        GetComponent<NetworkMecanimAnimator>().Animator.SetBool("IsOpen", false);
    }
}
