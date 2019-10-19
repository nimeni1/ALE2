using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALE2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Parser parser;
        StringChecker stringChecker;
        RegexConverter regexConverter;
        AutomatonConverter automatonConverter;
        AutomatonChecker automatonChecker;
        Automaton automaton;
        PDA pdaAutomaton;
        PDAparser pdaParser;

        void parse()
        {
            try
            {
                this.parser = new Parser();
                this.automaton = this.parser.handleAutomaton();
                this.DfaLabel.Content = "DFA Flag: ";
                this.DfaLabel.Content += this.automaton.dfaFlag.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("No automaton selected");
            }
        }

        void parsePDA()
        {
            try
            {
                this.pdaParser = new PDAparser();
                this.pdaAutomaton = this.pdaParser.readFromFile();
            }
            catch (Exception)
            {
                MessageBox.Show("No automaton selected");
            }
        }

        void checkString(string stringToCheck)
        {
            try
            {
                bool accepted;
                this.stringChecker = new StringChecker(this.automaton, stringToCheck);
                accepted = this.stringChecker.checkString();
                this.AcceptedStringLabel.Content = "Accepted string flag: ";
                this.AcceptedStringLabel.Content += accepted.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("No automaton selected");
            }
        }

        void checkPDAString(string stringToCheck)
        {
            //try
            //{
                bool accepted;
                accepted = this.pdaAutomaton.checkString(stringToCheck);
                this.PDAStringLabel.Content = "Accepted PDA string flag: ";
                this.PDAStringLabel.Content += accepted.ToString();
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("No automaton selected");
            //}
        }

        void convertString()
        {
            this.regexConverter = new RegexConverter();
            this.automaton = this.regexConverter.createAutomaton(this.ConvertStringTextBox.Text, 0);
        }

        void convertAutomaton()
        {
            //try
            //{
                if (this.automaton.dfaFlag == false)
                {
                    this.automatonConverter = new AutomatonConverter(this.automaton);
                    this.automaton.eliminateSilentTransitions();
                    this.automatonConverter.determineNewStates();
                    //this.automatonConverter.squashDuplicates();
                    //this.automatonConverter.reallocateTransitions();
                    //this.automatonConverter.convert();
                }
                else
                {
                    MessageBox.Show("Automaton is already DFA");
                }
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("No automaton selected");
            //}
        }

        void checkAutomaton()
        {
            try
            {
                bool finiteFlag;
                this.automatonChecker = new AutomatonChecker(this.automaton);
                this.automatonChecker.initializeCheck();
                finiteFlag = this.automatonChecker.check();
                this.FiniteWordsLabel.Content = "Finite words flag: ";
                this.FiniteWordsLabel.Content += finiteFlag.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("No automaton selected. Please parse an automaton file first");
            }

        }

        private void ParseBtn_Click(object sender, RoutedEventArgs e)
        {
            parse();
        }

        private void CheckStringBtn_Click(object sender, RoutedEventArgs e)
        {
            checkString(CheckStringTextBox.Text);
        }

        private void ConvertStringBtn_Click(object sender, RoutedEventArgs e)
        {
            convertString();
        }

        private void ConvertAutomatonBtn_Click(object sender, RoutedEventArgs e)
        {
            convertAutomaton();
        }

        private void CheckAutomatonBtn_Click(object sender, RoutedEventArgs e)
        {
            checkAutomaton();
        }

        private void ParsePDABtn_Click(object sender, RoutedEventArgs e)
        {
            parsePDA();
        }

        private void CheckPDAStringBtn_Click(object sender, RoutedEventArgs e)
        {
            checkPDAString(this.CheckPDAStringTextBox.Text);
        }
    }
}
