using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Sddc.SampleDataFlowComponent
{
    /// <summary>
    /// A sample data flow component, that can add talk specific columns.
    /// </summary>
    [DtsPipelineComponent(ComponentType = ComponentType.Transform,
        CurrentVersion = CURRENT_VERSION, Description = "A sample component that shows SSIS functionality.",
        DisplayName = "SDDC Sample Data Flow Component", IconResource = "Sddc.SampleDataFlowComponent.Resources.Icon1.ico",
        UITypeName = "Sddc.SampleDataFlowComponent.SddcSampleDataFlowComponentUI, Sddc.SampleDataFlowComponent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b82417df439a2b94")]
    public class SddcSampleDataFlowComponent : PipelineComponent
    {
        /// <summary>
        /// Current version of the component.
        /// </summary>
        const int CURRENT_VERSION = 2;

        /// <summary>
        /// Buffer index for the speaker input column.
        /// </summary>
        private int speakerInputColumnIndex;

        /// <summary>
        /// Buffer indexes for the output columns.
        /// </summary>
        private Dictionary<string, int> outputColumnIndexes;

        /// <summary>
        /// A program reader for reading the talk information.
        /// </summary>
        private ProgramReader.ProgramReader programReader;

        /// <summary>
        /// Provides the component properties when user
        /// drags the component to the SSIS package.
        /// </summary>
        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            
            SetComponentMetadata();
            CreateComponentProperties();
            InitInputAndOutput();

            ComponentMetaData.Version = CURRENT_VERSION;
        }

        /// <summary>
        /// Creates the component properties.
        /// <see cref="IDTSCustomPropertyCollection100">CustomPropertyCollection</see> is used
        /// to define a website address property.
        /// </summary>
        private void CreateComponentProperties()
        {
            var websiteProperty = ComponentMetaData.CustomPropertyCollection.New();
            websiteProperty.Name = "Website Address";
            websiteProperty.Description = "Address of the program website";
            websiteProperty.Value = String.Empty;
        }

        /// <summary>
        /// Sets up component metadata.
        /// </summary>
        private void SetComponentMetadata()
        {
            ComponentMetaData.ContactInfo = "Thomas Worm <Thomas.Worm@DATEV.DE>";
            ComponentMetaData.Description = "A sample component that shows SSIS functionality.";
        }

        /// <summary>
        /// Performs the upgrade if component version is higher.
        /// </summary>
        /// <param name="pipelineVersion">The current version of the component.</param>
        public override void PerformUpgrade(int pipelineVersion)
        {
            if (ComponentMetaData.Version < 2)
            {
                InitInputAndOutput();
            }

            ComponentMetaData.Version = CURRENT_VERSION;
        }

        /// <summary>
        /// Inits the input and the output.
        /// </summary>
        private void InitInputAndOutput()
        {
            var stdInput = InitInput();
            InitOutput(stdInput);
        }

        /// <summary>
        /// Inits the component output.
        /// SynchronousInputID is set for a synchronous data processing.
        /// The output columns title, description, begin and end are defined
        /// by adding them to the collection.
        /// </summary>
        /// <param name="stdInput">The components input.</param>
        private void InitOutput(IDTSInput100 stdInput)
        {
            var stdOutput = ComponentMetaData.OutputCollection[0];

            stdOutput.SynchronousInputID = stdInput.ID;

            var outputColumns = stdOutput.OutputColumnCollection;

            var titleColumn = outputColumns.New();
            titleColumn.Name = "Title";
            titleColumn.Description = "The titlt of the talk.";
            titleColumn.SetDataTypeProperties(DataType.DT_WSTR, 250, 0, 0, 0);

            var descriptionColumn = outputColumns.New();
            descriptionColumn.Name = "Description";
            descriptionColumn.Description = "The description of the talk.";
            descriptionColumn.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);

            var beginColumn = outputColumns.New();
            beginColumn.Name = "Begin";
            beginColumn.Description = "The start time of the talk.";
            beginColumn.SetDataTypeProperties(DataType.DT_DBTIMESTAMP, 0, 0, 0, 0);

            var endColumn = outputColumns.New();
            endColumn.Name = "End";
            endColumn.Description = "The end of the talk.";
            endColumn.SetDataTypeProperties(DataType.DT_DBTIMESTAMP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Inits the component input.
        /// <see cref="IDTSExternalMetadataColumnCollection100">External Metadata</see> is used
        /// to define the expected columns.
        /// </summary>
        /// <returns>The components input.</returns>
        private IDTSInput100 InitInput()
        {
            var stdInput = ComponentMetaData.InputCollection[0];

            stdInput.Name = "Eingabe mit Referenten";

            var externalMetadata = stdInput.ExternalMetadataColumnCollection;

            externalMetadata.IsUsed = true;

            var speakerColumn = externalMetadata.New();
            speakerColumn.Name = "Speaker";
            speakerColumn.Description = "Speaker for which the talk should be look up";
            speakerColumn.DataType = DataType.DT_WSTR;
            speakerColumn.Length = 100;
            return stdInput;
        }

        /// <summary>
        /// Validates the component state and fires
        /// error for more information.
        /// </summary>
        /// <returns>Returns the state of the component.</returns>
        public override DTSValidationStatus Validate()
        {
            bool cancel = false;

            if (ComponentMetaData.InputCollection.Count != 1)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "The component should have at least and only one input!",
                    "", 0, out cancel);
                return DTSValidationStatus.VS_ISCORRUPT;
            }

            if (ComponentMetaData.OutputCollection.Count != 1)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "The component should have at least and only one output!",
                    "", 0, out cancel);
                return DTSValidationStatus.VS_ISCORRUPT;
            }

            var websiteAdress = ComponentMetaData.CustomPropertyCollection["Website Address"].Value;
            if (String.IsNullOrEmpty(websiteAdress))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "The website address should be set!",
                    "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            try
            {
                new Uri(websiteAdress);
            }
            catch (Exception)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "The uri is not in correct format!",
                    "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            var externalSpeakerColumn = ComponentMetaData.InputCollection[0].ExternalMetadataColumnCollection["Speaker"];
            var speakerInputColumn =
                ComponentMetaData.InputCollection[0].InputColumnCollection.Cast<IDTSInputColumn100>()
                    .FirstOrDefault(c => c.ExternalMetadataColumnID == externalSpeakerColumn.ID);
            if (speakerInputColumn == null)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name, "There is no speaker input column mapped!",
                    "", 0, out cancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            return base.Validate();
        }

        /// <summary>
        /// Prepares the component before execution.
        /// For example the program website is parsed.
        /// </summary>
        public override void PreExecute()
        {
            DetermineBufferIndexes();
            ParseConferenceProgram();
        }

        /// <summary>
        /// Determines the indexes of the buffer.
        /// </summary>
        private void DetermineBufferIndexes()
        {
            var stdInput = ComponentMetaData.InputCollection[0];
            var externalSpeakerColumn = stdInput.ExternalMetadataColumnCollection["Speaker"];

            var speakerInputColumn =
                stdInput.InputColumnCollection.Cast<IDTSInputColumn100>()
                    .FirstOrDefault(c => c.ExternalMetadataColumnID == externalSpeakerColumn.ID);

            speakerInputColumnIndex = BufferManager.FindColumnByLineageID(stdInput.Buffer, speakerInputColumn.LineageID);

            var stdOutputColumns = ComponentMetaData.OutputCollection[0].OutputColumnCollection;
            outputColumnIndexes = new Dictionary<string, int>()
            {
                {"Title", BufferManager.FindColumnByLineageID(stdInput.Buffer, stdOutputColumns["Title"].LineageID)},
                {"Description", BufferManager.FindColumnByLineageID(stdInput.Buffer, stdOutputColumns["Description"].LineageID)},
                {"Begin", BufferManager.FindColumnByLineageID(stdInput.Buffer, stdOutputColumns["Begin"].LineageID)},
                {"End", BufferManager.FindColumnByLineageID(stdInput.Buffer, stdOutputColumns["End"].LineageID)}
            };
        }

        /// <summary>
        /// Parses the conference program.
        /// </summary>
        private void ParseConferenceProgram()
        {
            string websiteAddress = ComponentMetaData.CustomPropertyCollection["Website Address"].Value;
            programReader = new ProgramReader.ProgramReader(websiteAddress);
            programReader.Parse();
        }

        /// <summary>
        /// Proesses the buffer chunks.
        /// </summary>
        /// <param name="inputID">The ID of the input.</param>
        /// <param name="buffer">The buffer chunk.</param>
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            while (buffer.NextRow())
            {
                ProcessInputRow(buffer);
            }
        }

        /// <summary>
        /// Processes a row in the input.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        private void ProcessInputRow(PipelineBuffer buffer)
        {
            string speaker = buffer.GetString(speakerInputColumnIndex);
            var talk = programReader.ConferenceTalks.FirstOrDefault(t => t.Speaker.Contains(speaker));
            if (talk != null)
            {
                buffer.SetString(outputColumnIndexes["Title"], talk.Title);
                buffer.SetString(outputColumnIndexes["Description"], talk.Description);
                buffer.SetDateTime(outputColumnIndexes["Begin"], talk.Begin);
                buffer.SetDateTime(outputColumnIndexes["End"], talk.End);
            }
        }
    }
}
