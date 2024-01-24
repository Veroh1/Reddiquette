<template>
    <v-main>
        <profile-info-section />
    </v-main>
    <v-sheet>
        <side-bar />
    </v-sheet>
</template>

<script>
import ProfileInfoSection from '../components/ProfileInfoSection.vue';
import SideBar from '../components/SideBar.vue';

export default {
    components: {
        ProfileInfoSection,
        SideBar,
    }
}

</script>

<style>

</style>