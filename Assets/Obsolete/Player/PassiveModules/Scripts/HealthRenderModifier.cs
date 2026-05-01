using UnityEngine;

public class HealthRenderModifier : Module
{
    [SerializeField] private bool _disable_MM_Label;
    [SerializeField] private bool _customGameHealthRender;
    [SerializeField] private Sprite _customHealthIcon;
    [SerializeField] private Color _customHealthFillColor;
    [SerializeField] private Color _customHealthLabelColor;

    public override void MissionMenuLoad()
    {
        NanomachinesLabel nml = GameObject.FindObjectOfType<NanomachinesLabel>();

        if (_disable_MM_Label)
        {
            nml.Disconnect();
        } else if (_customGameHealthRender)
        {
            nml.SetIcon(_customHealthIcon);
            nml.SetFillColor(_customHealthFillColor);
            nml.SetTextColor(_customHealthLabelColor);
        }
    }

    public override void Load()
    {
        NanomachinesLabel nml = GameObject.FindObjectOfType<NanomachinesLabel>();
        
        if (_customGameHealthRender)
        {
            nml.SetIcon(_customHealthIcon);
            nml.SetFillColor(_customHealthFillColor);
            nml.SetTextColor(_customHealthLabelColor);
        }
    }
}
