
# Muse + Sonic Pi in VR environment.

This basic example using <a title="Muse Headband" href="http://www.choosemuse.com/">Muse Headband</a> and <a title="Sonic Pi" href="http://sonic-pi.net/">Sonic Pi</a>.
Subject supplied with visual and auditory feedback on EEG data.
Target EEG metric is mapped to audio.

Both Muse and Sonic Pi support OSC, which is used here for local network.
( WebGL build is done with socket.io, NodeJS and redis pubsub. )

Subject's viewport is rotating to keep focus on the current data.
Subject can change audio-sampler and audio-feedback rate.
Observers can move around freely and mark insightful events.

This example is a part of <a href="http://www.arcta.me/projects/cogsci-VR/">CogSci in VR project</a>.
