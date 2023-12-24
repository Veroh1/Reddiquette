<template>
    <v-main>
        <my-profile-section />
    </v-main>
    <side-bar />
</template>

<script>
import MyProfileSection from '../components/MyProfileSection.vue';
import SideBar from '../components/SideBar.vue';

export default {
    components: {
        MyProfileSection,
        SideBar,
    }
}

</script>

<style>

</style>