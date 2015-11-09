define(["sitecore"], function(Sitecore) {
    return Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor(
        "BaseLayouts.GetDialogUrl",
        function(response) {
            response.context.currentContext.value = response.responseValue.value;
        });
});