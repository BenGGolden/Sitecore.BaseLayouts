define(["myExperienceEditor"], function(experienceEditor) {
    return experienceEditor.PipelinesUtil.generateRequestProcessor(
        "BaseLayouts.GetDialogUrl",
        function(response) {
            response.context.currentContext.value = response.responseValue.value;
        });
});