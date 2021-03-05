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
using System.Windows.Threading;
using Wirepas_ELA_Mesh_Sample.Communication;
using Wirepas_ELA_Mesh_Sample.Model;

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
        private SampleMqttClient m_client = null;

        /** \brief current state of the workflow */
        private bool m_bWorkflowStarted = false;

        /** \brief internal current broker informations */
        private MqttBrokerInfo m_currentBrokerInfos = null;

        /** \brief intenral current subscription broker informations */
        private MqttSubscribeInfo m_currentSubscribeInformation = null;

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
            this.btnManageWorkflow.Content = DISPLAY_START_WF;
            this.Closing += MainWindow_Closing;
            //
            this.tbBrokerTopicInput.Text = MQTT_MAIN_TOPIC;
        }

        /**
         * \fn checkInputParameters
         * \brief function to test and validate the input parameters
         * \return parameters are valid (true) or not (false)
         */
        private bool checkInputParameters()
        {
            if(true == String.IsNullOrEmpty(this.tbBrokerHostnameInput.Text))
            {
                displayMessageError("Invalid IP Address / hostname input");
                return false;
            }
            if (true == String.IsNullOrEmpty(this.tbBrokerPortInput.Text))
            {
                displayMessageError("Invalid Port input");
                return false;
            }
            int port = 0;
            if(false == int.TryParse(this.tbBrokerPortInput.Text, out port))
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
        private void changeStateMqttFlow(bool state)
        {
            if(true == state)
            {
                this.listConsole.Items.Clear();
                //
                this.m_currentSubscribeInformation = new MqttSubscribeInfo(new string[] { MQTT_MAIN_TOPIC }, new byte[] { 0x00 });
                int port = 0;
                if (true == int.TryParse(this.tbBrokerPortInput.Text, out port))
                {
                    this.m_currentBrokerInfos = new MqttBrokerInfo(this.tbBrokerHostnameInput.Text, port);
                    //
                    this.m_client = new SampleMqttClient(this.m_currentBrokerInfos, false, null, null, uPLibrary.Networking.M2Mqtt.MqttSslProtocols.None);
                    this.m_client.Connect();
                    this.m_client.Subscribe(this.m_currentSubscribeInformation, SampleMqttClient_MqttMsgPublishReceived);
                    //
                    this.btnManageWorkflow.Content = DISPLAY_STOP_WF;
                    this.listConsole.Items.Add($"=========> : {DISPLAY_START_WF}");
                }
            }
            else
            {
                this.m_client.Unsubscribe(this.m_currentSubscribeInformation, SampleMqttClient_MqttMsgPublishReceived);
                this.m_client.Disconnect();
                this.m_client = null;
                //
                this.m_currentSubscribeInformation = null;
                this.m_currentBrokerInfos = null;
                //
                this.btnManageWorkflow.Content = DISPLAY_START_WF;
                this.listConsole.Items.Add($"=========> : {DISPLAY_STOP_WF}");
            }
            this.m_bWorkflowStarted = state;
        }
        #endregion

        #region event declaration and management
        /**
         * \fn btnManageWorkflow_Click
         * \brief event rised when button clicked
         */
        private void btnManageWorkflow_Click(object sender, RoutedEventArgs e)
        {
            if(checkInputParameters())
            {
                changeStateMqttFlow(!this.m_bWorkflowStarted);
            }
        }

        /**
         * \fn SampleMqttClient_MqttMsgPublishReceived
         * \brief callback associated to the subsciption event
         */
        private void SampleMqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            object decoded_message = ElaWirepasLibrary.Model.Wirepas.Payload.WirepasPayloadFactory.GetInstance().Get(e.Message);
            //
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                if (null != decoded_message)
                {
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(decoded_message);
                    this.listConsole.Items.Add($"Data Received : {jsonString}");
                }
            }));
        }

        /**
         * \fn MainWindow_Closing
         * \brief form closing ;=: need to use to ensure that client is closed
         */
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.m_bWorkflowStarted)
            {
                changeStateMqttFlow(false);
            }
        }
        #endregion
    }
}
