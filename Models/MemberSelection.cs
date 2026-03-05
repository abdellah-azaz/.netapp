using CommunityToolkit.Mvvm.ComponentModel;

namespace MonAppMultiplateforme.Models;

public partial class MemberSelection : ObservableObject
{
    public Member Member { get; }
    
    [ObservableProperty]
    private bool _isSelected;

    public MemberSelection(Member member)
    {
        Member = member;
    }
}
