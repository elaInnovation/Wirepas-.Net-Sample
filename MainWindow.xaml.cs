using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ElaWirepasLibrary.MQTT;

/**
 * \namspace Wirepas_ELA_Mesh_Sample
 * \bried namespace of the ELA mesh sammple
 */
namespace Wirepas_ELA_Mesh_Sample
{
    /**
     * \class MainWindow 
     * \brief application main windows 
     */
    public partial class MainWindow : Window
    {
        // constant definition
        private const String DISPLAY_START_WF = "Start Workflow";
        private const String DISPLAY_STOP_WF = "Stop Workflow";
        private const String MQTT_MAIN_TOPIC = "#";

        /** \brief internal client declaration */
        private WirepasMqttController m_mqttController = null;

        /** \brief current state of the workflow */
        private bool m_bWorkflowStarted = false;

        /** \brief constructor */
        public MainWindow()
        {
            InitializeComponent();
            initializeInternalComponents();
        }

        #region private methods declaration
        /**
         * \fn initializeInternalComponents
         * \brief function to initialize the different component and communication to get the flow from MQTT and Blue MESH
         */
        private void initializeInternalComponents()
        {
            btnManageWorkflow.Content = DISPLAY_START_WF;
            Closing += async (s, e) => await MainWindow_Closing(s, e);
        }

        /**
         * \fn checkInputParameters
         * \brief function to test and validate the input parameters
         * \return parameters are valid (true) or not (false)
         */
        private bool checkInputParameters()
        {
            if(true == String.IsNullOrEmpty(tbBrokerHostnameInput.Text))
            {
                displayMessageError("Invalid IP Address / hostname input");
                return false;
            }
            if (true == String.IsNullOrEmpty(tbBrokerPortInput.Text))
            {
                displayMessageError("Invalid Port input");
                return false;
            }
            int port = 0;
            if(false == int.TryParse(tbBrokerPortInput.Text, out port))
            {
                displayMessageError("Invalid Port input");
                return false;
            }
            return true;
        }

        /**
         * \fn displayMessageError
         * \brief display a message error
         * \param [in] message : input message to display to user
         */
        private void displayMessageError(String message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /**
         * \fn changeStateMqttFlow
         * \brief change the state of the mqtt workflow
         * \param [in] state : target input state
         */
        private async Task changeStateMqttFlow(bool state)
        {
            try
            {
                if (true == state)
                {
                    listConsole.Items.Clear();

                    if (int.TryParse(tbBrokerPortInput.Text, out var port))
                    {
                        m_mqttController = new WirepasMqttController(tbBrokerHostnameInput.Text, port);
                        await m_mqttController.ConnectAsync(tbBrokerUsernameInput.Text, tbBrokerPasswordInput.Password, cbBrokerTlsInput.IsChecked.GetValueOrDefault(), tbBrokerCertInput.Text);
                        m_mqttController.evWirepasDataReceived += M_mqttController_evWirepasDataReceived;
                        await m_mqttController.StartListeningToNodeDataAsync();
                        //
                        btnManageWorkflow.Content = DISPLAY_STOP_WF;
                        WriteLine($"=========> : {DISPLAY_START_WF}");
                    }
                }
                else
                {
                    m_mqttController.evWirepasDataReceived -= M_mqttController_evWirepasDataReceived;
                    await m_mqttController.DisconnectAsync();
                    m_mqttController = null;
                    //
                    btnManageWorkflow.Content = DISPLAY_START_WF;
                    WriteLine($"=========> : {DISPLAY_STOP_WF}");
                }
                m_bWorkflowStarted = state;
            }
            catch (Exception ex)
            {
                await Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    WriteLine($"Exception: {ex.Message}");
                }));
            }
        }

        private void M_mqttController_evWirepasDataReceived(WirepasMqttController sender, ElaTagClassLibrary.ElaTags.Interoperability.ElaBaseData device)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                WriteLine("");
                WriteLine($"Data Received : {device.identification.nodeAddress}");
                if (device?.sensor != null) WriteLine($"\t{device.sensor}");
                if (device?.diagnostic != null) WriteLine($"\t{device.diagnostic.batteryVoltage}");
                if (device?.localization != null) WriteLine(($"\t{string.Join(", ", device.localization.rawDataPoints.Select(p => $"{p.provider.anchorNodeAddress}: {p.rssi} dB"))}"));
            }));
        }
        #endregion

        #region event declaration and management
        /**
         * \fn btnManageWorkflow_Click
         * \brief event rised when button clicked
         */
        private async void btnManageWorkflow_Click(object sender, RoutedEventArgs e)
        {
            if(checkInputParameters())
            {
                await changeStateMqttFlow(!m_bWorkflowStarted);
            }
        }

        /**
         * \fn MainWindow_Closing
         * \brief form closing ;=: need to use to ensure that client is closed
         */
        private async Task MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(m_bWorkflowStarted)
            {
                await changeStateMqttFlow(false);
            }
        }
        #endregion

        #region console
        private void WriteLine(string line)
        {
            listConsole.Items.Add(line);
            listConsole.ScrollIntoView(listConsole.Items[listConsole.Items.Count - 1]);
        }
        #endregion
    }
}
