#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using System.Timers;
using FTOptix.WebUI;
#endregion

public class RuntimeNetLogic1 : BaseNetLogic
{
    private static System.Timers.Timer timer;
    // logic
    private MomentaryButton m1_button;
    private MomentaryButton m2_button;
    private IUAVariable active;
    private IUAVariable long_press;
    // eye candy
    private ScaleLayout pipe1;
    private ScaleLayout pipe2;
    private IUAVariable pupmp_flow;
    private IUAVariable pipe1_flow;
    private IUAVariable pipe2_flow;
    private IUAVariable tank_level;
    private IUAVariable tank_color;

    private Color Red = new Color(255, 255, 0, 0);
    private Color Yellow = new Color(255, 255, 255, 0);
    private Color Blu = new Color(255, 0, 174, 239);

    public override void Start()
    {
        // logic
        m1_button = Owner.Get<MomentaryButton>("MomentaryButton1");
        m2_button = Owner.Get<MomentaryButton>("MomentaryButton2");
        long_press = Project.Current.Get<Folder>("Model").Get<IUAVariable>("LongPress");
        active = Project.Current.Get<Folder>("Model").Get<IUAVariable>("Active");

        // eye candy
        pipe1 = Owner.Get<ScaleLayout>("PipeStraight1");
        pipe2 = Owner.Get<ScaleLayout>("Pipe90Degree1");
        pupmp_flow = Owner.Get<ScaleLayout>("Pump1").Get<IUAVariable>("ShowFlow");
        pipe1_flow = pipe1.Get<IUAVariable>("ShowFlow");
        pipe2_flow = pipe2.Get<IUAVariable>("ShowFlow");
        tank_level = Owner.Get<ScaleLayout>("TankStorage1").Get<IUAVariable>("Level");
        tank_color = Owner.Get<ScaleLayout>("TankStorage1").Get<IUAVariable>("LevelColor");

        timer = new System.Timers.Timer(100);
        timer.Enabled = true;
        timer.Elapsed += OnTimedEvent;
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        // logic
        if (tank_level.Value >= 100.00)
            return;

        if (active.Value)
            tank_level.Value += 0.5;
        if (long_press.Value)
            tank_level.Value += 0.5;
        else if (m1_button.Active)
            tank_level.Value += 0.5;
        else if (m2_button.Active)
            tank_level.Value += 0.5;

        // eye candy
        if (active.Value || long_press.Value || m1_button.Active || m2_button.Active)
        {
            pupmp_flow.Value = true;
            pipe1_flow.Value = true;
            pipe2_flow.Value = true;
        }
        else
        {
            pupmp_flow.Value = false;
            pipe1_flow.Value = false;
            pipe2_flow.Value = false;
        }

        if (tank_level.Value <= 70.0)
            tank_color.Value = Blu;
        else if (tank_level.Value <= 90.0)
            tank_color.Value = Yellow;
        else if (tank_level.Value <= 100.0)
            tank_color.Value = Red;
    }

    public override void Stop()
    {
    }

}
