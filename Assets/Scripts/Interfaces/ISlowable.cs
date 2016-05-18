using System.Collections;

public interface ISlowable
{
    void GetSlowed(float dur, float ratio);
    IEnumerator SlowCooldown(float time);
}