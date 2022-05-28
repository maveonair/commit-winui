using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Commit.Desktop.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(CommitCommand))]
        private string message;

        private string _commitEditMessageFilePath;
        public string CommitEditMessageFilePath => _commitEditMessageFilePath ??= GetCommitEditMessageFilePath();

        public IRelayCommand AbortCommand { get; }

        public IAsyncRelayCommand CommitCommand { get; }

        public MainViewModel()
        {
            AbortCommand = new RelayCommand(Abort);
            CommitCommand = new AsyncRelayCommand(Commit, CanCommit);
            Message = ReadCommitMessage();
        }

        private string GetCommitEditMessageFilePath()
        {
            var arg = Environment.GetCommandLineArgs().LastOrDefault();
            if (arg == null)
            {
                return string.Empty;
            }

            return !arg.Contains("COMMIT_EDITMSG") ? string.Empty : arg;
        }

        private void Abort()
        {
            App.Current.Exit();
        }

        private async Task Commit()
        {
            try
            {
                var lines = Message.Split("\r");
                await File.WriteAllLinesAsync(CommitEditMessageFilePath, lines);
            }
            catch (Exception e)
            {
                // TODO: Error handling
            }

            App.Current.Exit();
        }

        private bool CanCommit() => !string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(CommitEditMessageFilePath);

        private string ReadCommitMessage()
        {
            if (string.IsNullOrWhiteSpace(CommitEditMessageFilePath))
            {
                return string.Empty;
            }

            var content = File.ReadAllLines(CommitEditMessageFilePath);
            return string.Join("\n", content);
        }
    }
}
