using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using SpeechToSpeech.Models;
using SpeechToSpeech.Services;

namespace SpeechToSpeech.Repositories
{
  public class TextToSpeechRepository : ITextToSpeechRepository
  {
    private ISettingsService settingsService;
    private Settings settings;
    private string ConnectionString
    {
      get
      {
        return string.Format("DataSource=\"{0}\";Max Database Size={1};", settings.generalSettings.Database, settings.databaseSettings.MaxDatabaseSize);
      }
    }

    public TextToSpeechRepository(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
    }

    public async Task<bool> Delete(int id)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.DeleteAsync(new TextToSpeech { Id = id });
      }
    }

    public async Task<bool> DeleteByAudioFile(string audioFile)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        var result = await cnn.ExecuteAsync("delete from texttospeech where AudioFile = @AudioFile", new { AudioFile = audioFile});
        return result == 1;
      }
    }

    public async Task<TextToSpeech> Get(int id)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.GetAsync<TextToSpeech>(id);
      }
    }

    public async Task<List<TextToSpeech>> GetAll()
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        var results = await cnn.GetAllAsync<TextToSpeech>();
        return results.ToList();
      }
    }

    public async Task<List<TextToSpeech>> GetPage(int page, int amount)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        var startingRow = (page - 1) * amount + 1;
        var endingRow = startingRow + amount;
        var results = await cnn.QueryAsync<TextToSpeech>($@"
            SELECT  *
            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id ) AS RowNum, *
                      FROM      Orders
                    ) AS RowConstrainedResult
            WHERE   RowNum >= @RowStart
                AND RowNum < @RowEnd
            ORDER BY RowNum", new { RowStart = startingRow, RowEnd = endingRow });
        return results.ToList();
      }
    }

    public async Task<int> Insert(TextToSpeech textToSpeech)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.InsertAsync(textToSpeech);
      }
    }
  }
}
