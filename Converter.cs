using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class SectionData
{
    public List<List<float>> sectionNotes;
    public int lengthInSteps;
    public bool mustHitSection;
    public float bpm;
    public bool changeBPM;

} 

[Serializable]
public class SongDataOld
{
    public string song;
    public List<SectionData> notes;
    public float bpm;
    public bool needsVoices;
    public float speed;
    public string player1;
    public string player2;
	public string gfVersion;
	public int keys = 4; // todo: 4+ keys support
}

[Serializable]
public class SongPack
{
    public SongDataOld song;
}

[Serializable]
public class NoteData
{
    public float strumTime { get; set; }
    public int id { get; set; }
    public float length { get; set; }

} 

[Serializable]
public class SongData
{
    public string name { get; set; }
    public float bpm { get; set; }
    public float speed { get; set; }
	public int keys { get; set; } = 4;
    public List<NoteData> notes { get; set; }
}

public partial class Converter : Node2D
{
	public override void _Ready()
	{
		SongPack oldChart = loadFromJson("comatose");

		SongData newChart = new SongData();
		newChart.name = oldChart.song.song;
		newChart.bpm = oldChart.song.bpm;
		newChart.keys = oldChart.song.keys;
		newChart.speed = oldChart.song.speed * 0.45f;
		newChart.notes = new List<NoteData>();

		foreach (SectionData section in oldChart.song.notes)
		{
			foreach (List<float> note in section.sectionNotes)
			{
				NoteData newNote = new NoteData();
				newNote.strumTime = note[0];
				newNote.length = note[2];

				int id = (int)note[1];
				if (!section.mustHitSection)
				{
					switch (id)
					{
						case 0:
							id = 4;
							break;
						case 1:
							id = 5;
							break;
						case 2:
							id = 6;
							break;
						case 3:
							id = 7;
							break;
						case 4:
							id = 0;
							break;
						case 5:
							id = 1;
							break;
						case 6:
							id = 2;
							break;
						case 7:
							id = 3;
							break;
					}
				}
				newNote.id = id;

				newChart.notes.Add(newNote);
			}
		}

		saveNewJson("comatose", newChart);
	}

	public override void _Process(double delta)
	{
	}

	public static SongPack loadFromJson(string chartDir)
	{
		using FileAccess chartData = FileAccess.Open("res://" + chartDir + ".json", FileAccess.ModeFlags.Read);

		return JsonConvert.DeserializeObject<SongPack>(chartData.GetAsText());
	} 

	public static void saveNewJson(string chartDir, SongData content)
	{
		using FileAccess chartData = FileAccess.Open("res://" + chartDir + ".json", FileAccess.ModeFlags.Write);

		chartData.StoreString(JsonConvert.SerializeObject(content, Formatting.Indented));
	} 
}
