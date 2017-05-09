using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AgileCards
{
    public partial class MainPage : ContentPage
    {
        // Variable declaration.
        List<AgileCard> startColumn = new List<AgileCard>();
        List<AgileCard> progressColumn = new List<AgileCard>();
        List<AgileCard> endColumn = new List<AgileCard>();
        int currentCardID;
        int counter = 1;
        AgileCard previousCard;
        List<AgileCard> previousMoveFromColumn = new List<AgileCard>();
        List<AgileCard> previousMoveToColumn = new List<AgileCard>();

        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization On Page appearance.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LabelStart.Text = string.Empty;
            LabelProgress.Text = string.Empty;
            LabelEnd.Text = string.Empty;
            LabelCardNumber.Text = "1";
        }

        /// <summary>
        /// Function to perform on Move Next button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void OnNextButtonClicked(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(EntryCardToMove.Text) && PickerMoveToColumn.SelectedItem != null)
            {
                int cardToMove = Convert.ToInt32(EntryCardToMove.Text);
                string moveToColumn = PickerMoveToColumn.SelectedItem.ToString();
                List<AgileCard> listToMove = new List<AgileCard>();

                // Check which list to move to.
                switch (moveToColumn)
                {
                    case "Progress":
                        listToMove = progressColumn;
                        break;
                    case "End":
                        listToMove = endColumn;
                        break;
                }

                // Check which list the card exists in and move to desired list.
                if (startColumn.Exists(e => e.ID == cardToMove))
                {
                    AgileCard cardToMoveNext = startColumn.Find(e => e.ID == cardToMove);
                    listToMove.Add(cardToMoveNext);
                    startColumn.Remove(cardToMoveNext);
                    RefreshAllLists();
                    previousCard = cardToMoveNext;
                    previousMoveFromColumn = startColumn;
                    previousMoveToColumn = listToMove;
                }
                else if (progressColumn.Exists(e => e.ID == cardToMove))
                {
                    AgileCard cardToMoveNext = progressColumn.Find(e => e.ID == cardToMove);
                    listToMove.Add(cardToMoveNext);
                    progressColumn.Remove(cardToMoveNext);
                    RefreshAllLists();
                    previousCard = cardToMoveNext;
                    previousMoveFromColumn = progressColumn;
                    previousMoveToColumn = listToMove;
                }
                else if (endColumn.Exists(e => e.ID == cardToMove))
                {
                    // When the card is already ended, display following message.
                    await DisplayAlert("Message", "This card has ended. It can't be moved further", "OK");
                }
                else
                {
                    // Display error message when this card does not exist in the iteration.
                    await DisplayAlert("Message", "This card does not exist.", "OK");
                }
            }
            else
            {
                // Display error message when the required input values are not entered.
                await DisplayAlert("Message", "Enter Card To Move and the Column to move to", "OK");
            }
        }

        /// <summary>
        /// Function to perform on click of Undo Move button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnUndoButtonClicked(object sender, EventArgs args)
        {
            //Undo the previous move 
            if (previousCard != null && previousMoveFromColumn != null && previousMoveToColumn != null)
            {
                previousMoveToColumn.Remove(previousCard);
                previousMoveFromColumn.Add(previousCard);
                RefreshAllLists();
            }
            previousCard = null;
            previousMoveFromColumn = null;
            previousMoveToColumn = null;
        }

        /// <summary>
        /// Function to calculate velocity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void OnCalculateVelocityButtonClicked(object sender, EventArgs args)
        {
            int velocity = 0;
            if (endColumn.Count > 0)
            {
                foreach (var card in endColumn)
                {
                    velocity += card.Estimate;
                }
                await DisplayAlert("Velocity", "Velocity = " + velocity.ToString(), "OK");
            }
            else
            {
                await DisplayAlert("Message", "There are no cards in end state to calculate velocity.", "OK");
            }
        }

        /// <summary>
        /// Function to perform on click of create card button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnCreateButtonClicked(object sender, EventArgs args)
        {
            // Get input values for card from the user and create a card in the Start list.
            currentCardID = counter++;
            int estimate = string.IsNullOrEmpty(EntryEstimate.Text) ? 0 : Convert.ToInt32(EntryEstimate.Text);
            startColumn.Add(new AgileCard() { Title = EntryTitle.Text, Description = EntryDescription.Text, Estimate = estimate, ID = currentCardID });

            UpdateColumn(LabelStart, startColumn);
            LabelCardNumber.Text = (currentCardID + 1).ToString();

            EntryTitle.Text = string.Empty;
            EntryDescription.Text = string.Empty;
            EntryEstimate.Text = string.Empty;
        }

        /// <summary>
        /// Function to refresh all list values
        /// </summary>
        void RefreshAllLists()
        {
            UpdateColumn(LabelProgress, progressColumn);
            UpdateColumn(LabelStart, startColumn);
            UpdateColumn(LabelEnd, endColumn);
        }

        /// <summary>
        /// Function to update and display a specific column of the iteration
        /// </summary>
        /// <param name="labelToUpdate">Column Label To Update</param>
        /// <param name="columnToDisplay">List values to display</param>
        void UpdateColumn(Label labelToUpdate, List<AgileCard> columnToDisplay)
        {
            labelToUpdate.Text = string.Empty;
            foreach (var item in columnToDisplay)
            {
                labelToUpdate.Text = labelToUpdate.Text + "   " + item.ID.ToString();
            }
        }
    }
}
