define(["myExperienceEditor"], function(experienceEditor) {
    return experienceEditor.PipelinesUtil.generateRequestProcessor("BaseLayouts.Execute", function(response) {
        if (response.responseValue.value) {
            window.top.location.reload();
        }
    });
});