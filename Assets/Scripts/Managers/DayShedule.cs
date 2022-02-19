using System.Collections.Generic;

public class DayShedule
{
    private Clock Clock;
    private List<SheduledEvent> eventsList;

    public DayShedule(List<SheduledEvent> eventsL, Clock clock){
        eventsList = eventsL;
        Clock = clock;
    }

    public void CheckEvent(){
        if (eventsList != null)
            if (eventsList.Count > 0){
                float time = Clock.GetTime();
                if (time > eventsList[0].time){
                    eventsList[0].eventInvokeMethod?.Invoke();
                    if(eventsList[0].tagToAdd!=null)
                        TagManager.AddTag(eventsList[0].tagToAdd);
                    eventsList.RemoveAt(0);
                }
            }
    }
}