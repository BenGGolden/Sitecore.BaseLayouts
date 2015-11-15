define(["/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function(ExperienceEditor) {
    return ExperienceEditor.PipelinesUtil.generateRequestProcessor("BaseLayouts.Execute", function(response) {
        if (response.responseValue.value) {
            window.top.location.reload();
        }
    });
});