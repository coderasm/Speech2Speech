﻿using SpeechToSpeech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Repositories
{
  public interface ITextToSpeechRepository
  {
    Task<int> Insert(TextToSpeech textToSpeech);
    Task<bool> Delete(int id);
    Task<bool> DeleteByAudioFile(string audioFile);
    Task<TextToSpeech> Get(int id);
    Task<List<TextToSpeech>> GetAll();
    Task<List<TextToSpeech>> GetPage(int page, int amount);
  }
}
