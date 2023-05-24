using Avalonia.Controls;

namespace Blueway.Views
{
    public partial class Sources : AUC
    {
        public Sources()
        {
            InitializeComponent();
            Title = "Sources";
        }

        /*
          <StackPanel Orientation="Horizontal" Spacing="5">
						<Button Content="Delete Image here" />
						<StackPanel Orientation="Vertical">
							<StackPanel Orientation="Horizontal" Spacing="5">
								<TextBlock Text="Source Name Here" FontSize="20" />
								<Image Source="Source Icon here" Width="16" Height="16" />
							</StackPanel>
							<TextBlock Text="Source Description Here" />
						</StackPanel>
					</StackPanel>
         */
    }
}