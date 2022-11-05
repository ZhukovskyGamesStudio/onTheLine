public class VirtualTalkingBubble : TalkingBubble {
    protected override void PrintSymbol(char symbol) {
        curText.text += symbol;
    }
}