using Amazon.Polly;
using Google.Cloud.TextToSpeech.V1;
using System;
using System.ComponentModel;

namespace SpeechToSpeech.Models
{
  public class Voice: IEquatable<Voice>, INotifyPropertyChanged
  {
    public int Id { get; set; }
    public VoiceId VoiceId { get; set; }
    public string Gender { get; set; }
    public SsmlVoiceGender SsmlGender { get; set; }
    public string Language { get; set; }
    private string _name;
    public string Name {
      get
      {
        return _name;
      }
      set
      {
        if(_name != value)
        {
          _name = value;
          NotifyPropertyChanged("Name");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool Equals(Voice other)
    {
      if (other == null)
        return false;
      return string.Equals(other.Name, Name) &&
        //other.Id == Id &&
        string.Equals(other.Gender, Gender);
        //other.SsmlGender == SsmlGender;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals(obj as Voice);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = 13;
        var nameHashCode = !string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0;
        hashCode = (hashCode * 397) ^ nameHashCode;
        //var idHashCode = ReferenceEquals(null, Id) ? 0 : Id.GetHashCode();
        //hashCode = (hashCode * 397) ^ idHashCode;
        var genderHashCode = !string.IsNullOrEmpty(Gender) ? Gender.GetHashCode() : 0;
        hashCode = (hashCode * 397) ^ genderHashCode;
        //var ssmlGenderHashCode = ReferenceEquals(null, Id) ? 0 : SsmlGender.GetHashCode();
        //hashCode = (hashCode * 397) ^ ssmlGenderHashCode;
        return hashCode;
      }
    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
