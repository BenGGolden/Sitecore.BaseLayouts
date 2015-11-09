define(["sitecore"], function(Sitecore) {
    return Sitecore.ExperienceEditor.PipelinesUtil.generateRequestProcessor("BaseLayouts.Execute", function (response) {
        if (response.responseValue.value) {
            window.top.location.reload();
        }
    });
});