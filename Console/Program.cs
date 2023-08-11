using Console.Helpers;
using Spectre.Console;
using YoutubeExplode;
using YoutubeExplode.Playlists;

AnsiConsole.MarkupLine("[red]YoutubeMusic[/].[purple]Console[/]");
AnsiConsole.MarkupLine("[purple]Retrieving[/] cookies");

var storedCookies = CookieStore.GetCookies();
if (!CookieStore.ValidateCookies(storedCookies))
{
    AnsiConsole.MarkupLine("[red]Invalid[/] cookies");
    AnsiConsole.MarkupLine("[purple]Retrieving[/] new cookies");
    var cookies = CookieRetriever.GetCookies();
    if (cookies == null)
        throw new Exception("no cookies retrieved");
    if (!CookieStore.ValidateCookies(cookies))
        throw new Exception("retrieved cookies are invalid");

    CookieStore.SaveCookies(cookies);
    AnsiConsole.MarkupLine("[green]Saved[/] cookies");
}
else
    AnsiConsole.MarkupLine("[green]Valid[/] cookies");

var cookie = CookieStore.GetCookies();

var client = new YoutubeClient(cookie);
AnsiConsole.MarkupLine("[purple]Loading[/] playlist");
var playlist = client.Playlists.GetVideosAsync("LM");
int position = 0;
List<PlaylistVideo> videos = new List<PlaylistVideo>();

await foreach(var video in playlist)
    videos.Add(video);

AnsiConsole.MarkupLine($"[green]loaded[/] {videos.Count()} songs");

var cursor = System.Console.GetCursorPosition();
while (position >= 0 && position < videos.Count)
{
    System.Console.SetCursorPosition(cursor.Left, cursor.Top);
    var video = videos[position];
    var manifest = await client.Videos.Streams.GetManifestAsync(video.Id);
    var stream = manifest.GetAudioStreams().OrderByDescending(x => x.Bitrate).FirstOrDefault();
    if (stream == null)
    {
        position++; // Skip this video and move to the next
        continue;
    }

    AudioPlayer.Play(stream.Url, video.Duration);
    PlayEvent status = PlayEvent.None;

    AnsiConsole.Progress()
        .AutoClear(false)   // Do not remove the task list when done
        .HideCompleted(false)   // Hide tasks as they are completed
        .Columns(new ProgressColumn[]
        {
            new TaskDescriptionColumn(),    // Task description
            new ProgressBarColumn(),        // Progress bar
            new PercentageColumn(),         // Percentage
            new RemainingTimeColumn(),      // Remaining time
        })
        .Start(ctx =>
        {
            var task = ctx.AddTask($"{LimitString(video.Title)} [red]{position+1}[/]");

            while(status == PlayEvent.None)
            {
                Thread.Sleep(500);
                task.Value = AudioPlayer.Percent;
                status = AudioPlayer.Status();
            }
        });

    if (status == PlayEvent.Prev)
    {
        position = Math.Max(0, position - 1); // Move to the previous song, if possible
    }
    else
    {
        position++; // Move to the next song
        if(position >  videos.Count)
            position = 0;
    }
}


string LimitString(string value)
{
    if(value.Length <= 20)
        return value;
    return value.Substring(0,16) + "[gray]...[/]";
}